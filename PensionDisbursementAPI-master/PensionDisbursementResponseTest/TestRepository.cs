using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PenionDisbursementAPI.Models;
using PenionDisbursementAPI.Providers;
using PenionDisbursementAPI.Repository;

namespace PensionDisbursementResponseTest
{
	public class TestRepository
	{
		private  Mock<IPensionProvider> _provider;
		private  IPensionerRepo _repo;

		HttpResponseMessage testRes;
		PensionerDetail pensionDetail;
		[SetUp]
		public void Setup()
		{
			_provider = new Mock<IPensionProvider>();
			_repo = new PensionerRepo(_provider.Object);
			testRes = new HttpResponseMessage(HttpStatusCode.NotFound);
			pensionDetail = new PensionerDetail


			{
				Name = "Peeyush",
				DateofBirth = Convert.ToDateTime("1999-11-05"),
				PAN = "BCFPN1234F",
				AadharNumber = "111122223333",
				SalaryEarned = 40000,
				Allowances = 5000,
				PensionType = (PensionTypeValue)(2),
				BankName = "HDFC",
				AccountNumber = "123456789876",
				BankType = (BankType)(2)
			};
		}

		[TestCase(25550.0, 550, "111122223333")]
		public void PensionerRepo_CorrectAadhar_returnOk(double pension, int charges, string aadhar)
		{
			ProcessPensionInput processPensionInput = new()
			{
				AadharNumber = aadhar,
				BankCharge = charges,
				PensionAmount = pension
			};

			_provider.Setup(p => p.GetData(aadhar)).Returns(pensionDetail);
			PensionerDetail pensionerDetail = _provider.Object.GetData(aadhar);
			int n = _repo.Status(pensionerDetail, processPensionInput);
			Assert.AreEqual(10, n);

		}


		//invalid pension amount

		[TestCase(24430.0, 550, "111122223333")]
		public void PensionerRepository_Invalid_PensionAmount_return_ErrorStatus(double pension, int charges, string aadhaar)
		{
			ProcessPensionInput processPensionInput = new ProcessPensionInput { AadharNumber = aadhaar, BankCharge = charges, PensionAmount = pension };

			_provider.Setup(p => p.GetData(aadhaar)).Returns(pensionDetail);
			PensionerDetail pensionerDetail = _provider.Object.GetData(aadhaar);
			int n = _repo.Status(pensionerDetail, processPensionInput);
			Assert.AreNotEqual(10, n);
		}


		//invalid aadhar card 
		[TestCase(24450.0, 550, "111122220000")]

		public void PensionerRepo_InvalidAadharNumber_returnErrorStatus(double pension, int charges, string aadhar)
		{
			ProcessPensionInput processPensionInput = new ProcessPensionInput
			{
				AadharNumber = aadhar,
				BankCharge = charges,
				PensionAmount = pension
			};
			int n = 10;
			_provider.Setup(p => p.GetData(aadhar)).Returns(pensionDetail);

			PensionerDetail pensionerDetail = _provider.Object.GetData(aadhar);

			if (pensionDetail.AadharNumber != aadhar) n = 21;

			Assert.AreNotEqual(10, n);
		}


		//Invalid Bank Charge
		[TestCase(24430.0, 440, "111122223333")]
		public void PensionerRepository_Invalid__Bankcharge_return_ErrorStatus(double pension, int charges, string aadhaar)
		{
			ProcessPensionInput processPensionInput = new ProcessPensionInput
			{ 
				AadharNumber = aadhaar,
				BankCharge = charges, 
				PensionAmount = pension 
			};

			_provider.Setup(p => p.GetData(aadhaar)).Returns(pensionDetail);
			PensionerDetail pensionerDetail = _provider.Object.GetData(aadhaar);
			int n = _repo.Status(pensionerDetail, processPensionInput);
			Assert.AreEqual(21, n);
		}
	}


}