using CB_WebApiTask.Data;
using CB_WebApiTask.Models.Entities;
using CB_WebApiTask.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace CustomerRegistrationApi.Controllers;
/// <summary>
/// Author NB
/// Date: 11-27-2025
/// Purpose: API Controller For Sign In Request
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SignInController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    public SignInController(ApplicationDbContext db)
    {
        _dbContext = db;
    }

    //Start sign-in: validate IC
    [HttpPost("signinuser")]
    public async Task<IActionResult> SignIn([FromBody] SignInStartRequest req)
    {
        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer == null)
            return BadRequest(new { message = "Invalid user" });

        return Ok(new { message = "IC valid. Proceed to request OTP." });
    }
    //Request Otp
    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] SignInOtpRequest req)
    {
        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer == null)
            return BadRequest(new { message = "Invalid user" });

        if (req.DeliveryMethod.ToUpper() == "EMAIL" && string.IsNullOrWhiteSpace(customer.EmailAddress))
            return BadRequest(new { message = "Email not available for this user" });

        var recent = await _dbContext.OtpSessions
            .Where(x => x.IcNumber == req.IcNumber && x.Purpose == "SignIn")
            .OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();

        if (recent != null && (DateTime.UtcNow - recent.CreatedOn).TotalSeconds < 60)
            return StatusCode(429, new { message = "Please wait before requesting another OTP" });

        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        var otp = new OtpSession
        {
            IcNumber = req.IcNumber,
            MobileNumber = customer.MobileNumber,
            EmailAddress = customer.EmailAddress,
            Code = code,
            Purpose = "SignIn",
            DeliveryMethod = req.DeliveryMethod.ToUpper() == "EMAIL" ? "EMAIL" : "SMS",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        _dbContext.OtpSessions.Add(otp);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "OTP generated", delivery = otp.DeliveryMethod, otp = code });
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
        if (customer != null) customer.IsOtpVerified = true;

        await _dbContext.SaveChangesAsync();
        return Ok(new { verified = true });
    }

    //Privacy acceptance sign-in
    [HttpPost("privacy")]
    public async Task<IActionResult> AcceptPrivacy([FromBody] PrivacyRequest req)
    {
        if (!req.Accept) return BadRequest(new { message = "Privacy must be accepted" });

        var otpVerified = await _dbContext.OtpSessions.AnyAsync(x =>
            x.IcNumber == req.IcNumber && x.Purpose == req.Purpose && x.IsVerified && x.ExpiresAt >= DateTime.UtcNow.AddMinutes(-10));

        if (!otpVerified) return BadRequest(new { message = "OTP not verified" });

        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer == null) return BadRequest(new { message = "Customer not found" });

        customer.IsPrivacyAccepted = true;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Privacy accepted" });
    }

    //Create PIN
    [HttpPost("create-pin")]
    public async Task<IActionResult> CreatePin([FromBody] CreatePinRequest req)
    {
        if (req.Pin != req.ConfirmPin) return BadRequest(new { message = "PIN mismatch" });
        if (!req.Pin.All(char.IsDigit) || req.Pin.Length != 6) return BadRequest(new { message = "PIN invalid" });

        var customer = await _dbContext.Customers.FindAsync(req.IcNumber);
        if (customer == null) return BadRequest(new { message = "Customer not found" });
        if (!customer.IsPrivacyAccepted) return BadRequest(new { message = "Privacy must be accepted" });

        customer.PinHash = Hash(req.Pin);
        customer.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "PIN set / sign-in completed" });
    }

    private static string Hash(string value)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
