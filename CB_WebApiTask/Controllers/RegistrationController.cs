using CB_WebApiTask.Data;
using CB_WebApiTask.Models.Entities;
using CB_WebApiTask.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace CB_WebApiTask.Controllers;
/// <summary>
/// Author NB
/// Date: 11-27-2025
/// Purpose: API Controller For Registration Request
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    public RegistrationController(ApplicationDbContext db) 
    {
        _dbContext = db;
    }

    //Register New User 
    [HttpPost("register-form")]
    public async Task<IActionResult> RegisterForm([FromBody] RegisterFormRequest req)
    {        
        if (await _dbContext.Customers.AnyAsync(c => c.IcNumber == req.IcNumber || c.MobileNumber == req.MobileNumber))
            return BadRequest(new { message = "IC or Mobile already registered" });

        var customer = new Customer
        {
            IcNumber = req.IcNumber,
            CustomerName = req.CustomerName,
            MobileNumber = req.MobileNumber,
            EmailAddress = req.EmailAddress
        };
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();
        return Ok(new { message = "User details saved successfully" });
    }
    //Request OTP for registration
    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpRequest req)
    {
        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer == null)
            return BadRequest(new { message = "IC not found. Please register first." });

        if (customer.MobileNumber != req.MobileNumber)
            return BadRequest(new { message = "Mobile number does not match IC." });

        var recent = await _dbContext.OtpSessions
            .Where(x => x.IcNumber == req.IcNumber && x.Purpose == "Registration")
            .OrderByDescending(x => x.CreatedOn)
            .FirstOrDefaultAsync();

        if (recent != null && (DateTime.UtcNow - recent.CreatedOn).TotalSeconds < 60)
            return StatusCode(429, new { message = "Please wait before requesting another OTP" });

        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        _dbContext.OtpSessions.Add(new OtpSession
        {
            IcNumber = req.IcNumber,
            MobileNumber = req.MobileNumber,
            Code = code,
            Purpose = "Registration",
            DeliveryMethod = req.OtpDeliveryMethod,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        });
        await _dbContext.SaveChangesAsync();
        return Ok(new { message = "OTP generated", otp = code });

    }


    //Verify OTP
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest req)
    {
        var otp = await _dbContext.OtpSessions
            .Where(x => x.IcNumber == req.IcNumber &&
                        x.Purpose == req.Purpose &&
                        !x.IsVerified &&
                        x.ExpiresAt >= DateTime.UtcNow &&
                        x.Code == req.Otp)
            .OrderByDescending(x => x.CreatedOn)
            .FirstOrDefaultAsync();

        if (otp == null)
            return BadRequest(new { verified = false, message = "Invalid or expired OTP" });

        otp.IsVerified = true;
        await _dbContext.SaveChangesAsync();

        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer != null)
        {
            customer.IsOtpVerified = true;
            await _dbContext.SaveChangesAsync();
        }
        return Ok(new { verified = true });
    }

    //Privacy acceptance
    [HttpPost("privacy")]
    public async Task<IActionResult> AcceptPrivacy([FromBody] PrivacyRequest req)
    {
        if (!req.Accept) return BadRequest(new { message = "Privacy must be accepted" });

        // Verify that a recent OTP was verified
        var otpVerified = await _dbContext.OtpSessions.AnyAsync(x =>
            x.IcNumber == req.IcNumber && x.Purpose == req.Purpose && x.IsVerified && x.ExpiresAt >= DateTime.UtcNow.AddMinutes(-10));

        if (!otpVerified)
            return BadRequest(new { message = "OTP not verified" });

        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer == null)
        {
            customer = new Customer
            {
                IcNumber = req.IcNumber,
                CustomerName = string.Empty,
                MobileNumber = string.Empty,
                EmailAddress = string.Empty,
                IsOtpVerified = true,
                IsPrivacyAccepted = true
            };
            _dbContext.Customers.Add(customer);
        }
        else
        {
            customer.IsPrivacyAccepted = true;
            customer.IsOtpVerified = true;
        }

        await _dbContext.SaveChangesAsync();
        return Ok(new { message = "Privacy policy accepted" });
    }

    //Create PIN and complete registration
    [HttpPost("create-pin")]
    public async Task<IActionResult> CreatePin([FromBody] CreatePinRequest req)
    {
        if (req.Pin != req.ConfirmPin)
            return BadRequest(new { message = "PIN and Confirm PIN do not match" });

        if (!req.Pin.All(char.IsDigit) || req.Pin.Length != 6)
            return BadRequest(new { message = "PIN must be exactly 6 digits" });

        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer == null)
            return BadRequest(new { message = "Customer not found. Ensure privacy step completed." });

        if (!customer.IsPrivacyAccepted)
            return BadRequest(new { message = "Privacy policy must be accepted before setting PIN" });

        customer.PinHash = Hash(req.Pin);
        customer.IsFullyRegistered = true;
        await _dbContext.SaveChangesAsync();
        return Ok(new { message = "Customer registration completed" });
    }

    private static string Hash(string value)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    #region DevTest
    //Only For Testing Get All Users
    [HttpGet("all")]
    public async Task<IActionResult> GetAll() => Ok(await _dbContext.Customers.ToListAsync());

    //Delete Existing User
    [HttpDelete("delete/{icNumber}")]
    public IActionResult DeleteCustomer(string icNumber)
    {
        var customer = _dbContext.Customers
            .FirstOrDefault(c => c.IcNumber == icNumber);

        if (customer == null)
        {
            return NotFound(new { message = "Customer not found." });
        }

        var otps = _dbContext.OtpSessions
            .Where(o => o.IcNumber == icNumber)
            .ToList();
        _dbContext.OtpSessions.RemoveRange(otps);
        _dbContext.Customers.Remove(customer);
        _dbContext.SaveChanges();

        return Ok(new { message = "Customer deleted successfully." });
    }
    #endregion

}

