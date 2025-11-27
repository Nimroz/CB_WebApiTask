Database Backup:
The database backup (.bak file) is located in the /Scripts folder.
You can restore it using SQL Server Management Studio => Restore Database => Device => Add => Select file.

					****Api Calling Commands****
			For Registration
GET https://localhost:****/api/Registration/all
Response OK Gets All Users

POST https://localhost:****/api/Registration/register-form
Body
{
  "IcNumber": "12345678",
  "CustomerName": "John Doe",
  "MobileNumber": "03001234567",
  "EmailAddress": "john@example.com"
}
Response OK
{
  "message": "User created. Proceed to request OTP."
}

POST https://localhost:****/api/Registration/request-otp
Body
{
  "IcNumber": "12345678",
  "DeliveryMethod": "SMS"
}
Response OK
{
  "message": "OTP generated",
  "otp": "654321"(Random#) Copy OTP
}

POST https://localhost:****/api/Registration/verify-otp
Body
{
  "IcNumber": "1234567890",
  "Otp": "654321",  Paste OTP
  "Purpose": "Registration"
}
Response OK
{
  "verified": true
}
POST https://localhost:****/api/Registration/privacy
Body
{
  "IcNumber": "1234567890",
  "Accept": true,
  "Purpose": "Registration"
}
Response OK
{
  "message": "Privacy policy accepted"
}

POST https://localhost:****/api/Registration/create-pin
Body
{
  "IcNumber": "1234567890",
  "Pin": "123456",
  "ConfirmPin": "123456"
}
Response OK
{
  "message": "Customer registration completed"
}

For SignIn
POST https://localhost:****/api/SignIn/signinuser
Body
{
  "IcNumber": "1234567890"
}
Response OK
{
  "message": "IC valid. Proceed to request OTP."
}

POST https://localhost:****/api/SignIn/request-otp
Body
{
  "IcNumber": "1234567890",
  "DeliveryMethod": "SMS"
}
Response OK
{
  "message": "OTP generated",
  "otp": "654321"	(Random#) Copy OTP
}
POST https://localhost:****/api/SignIn/verify-otp
Body
{
  "IcNumber": "1234567890",
  "Otp": "654321",  Paste OTP
  "Purpose": "SignIn"
}
Response OK
{
  "verified": true
}

POST https://localhost:****/api/SignIn/privacy
Body
{
  "IcNumber": "1234567890",
  "Accept": true,
  "Purpose": "SignIn"
}
Response OK
{
  "message": "Privacy policy accepted"
}
POST https://localhost:****/api/SignIn/create-pin
Body
{
  "IcNumber": "1234567890",
  "Pin": "123456",
  "ConfirmPin": "123456"
}
Response OK
{
  "message": "PIN set / sign-in completed"
}

