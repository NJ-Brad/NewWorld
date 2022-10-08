using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewWorld
{
    internal class TestData
    {
        public static string GetWhamFlow()
        {
			return @"
			flow
			[
				' https://mermaid-js.github.io/mermaid/#/flowchart
			
				Start
			
				""Loan Type""
			
				""Home Description""
			
				""Property Use""
			
				decision ""Loan Type = Purchase?""
				[
					Yes ttp
			
					No d1
				]
			
				""Timeframe to Purchase"" ttp
			
				""First Time Home Buyer""
				[
					_ a10
				]
			
				decision ""Loan Type = Refinance?"" d1
				[
					Yes lprc
			
					No hv
				]
			
				""Loan Purpose Refi slash Cashout"" lprc
			
				""Home Value"" hv
			

				decision ""Loan Purpose = Refi slash Cashout"" d2
				[
					Yes mb
			
					No d3
				]
			
				""Mortgage Balance"" mb
			
				decision ""Loan Type = Cashout`OR Loan Purpose = Cashout Refi`OR Loan Purpose = Take Cash Out"" d3
				[
					Yes a9
			
					No a10
				]
			
				action ""Additional Funds"" a9
			
				""Military"" a10
			
				decision ""Loan Type = Purchase""
				[
					Yes a11
			
					No ""Credit Profile""
				]
			
				""Working w/Agent"" a11
			
				""Purchase Price""
			
				""Down Payment""
			
				""Credit Profile""
			
				decision ""Loan Type = Refi slash C.O."" ltrsco
				[
					Yes ""Second Mortgage""
			
					No qpv
				]
			
				""Second Mortgage""
			
				decision ""QPV = NoEmp"" qpv
				[
					No ""Employment Status""
			
					Yes d7
				]
			
				decision ""Loan Type = Purchase`And F.T.H.B. = No"" d7
				[
					No ""Bankruptcy""
			
					Yes ""Late Payments""
				]
			
				""Late Payments""
			
				""Foreclosure""
				[
					_ ""Bankruptcy""
				]
			
				""Employment Status""
			
				""Bankruptcy""
			
				""Name""
			
				""Home Phone""
			
				""Email""
			
				""Address and Zip"" anz
			
				action ""Submit Transaction"" a24
			'	sub ""Verify Results""
			
				decision ""Error"" d8
				[
					Yes anz
			
					No redirect
				]
			
				""Redirect to slash thank-you"" redirect
			
				End
			]
";
		}
		public static string GetNoDecisionWhamFlow()
		{
			return @"
			flow
			[
				' https://mermaid-js.github.io/mermaid/#/flowchart
			
				Start
			
				""Loan Type""
			
				""Home Description""
			
				""Property Use""
				[
					""if Loan Type = Purchase"" ttp
					""if Loan type = Refinance"" lprc
					""Other Loan type"" ""Home Value""
				]
			
				""Timeframe to Purchase"" ttp
			
				""First Time Home Buyer""
				[
					_ ""Military""
				]
			
				""Loan Purpose Refi slash Cashout"" lprc
			
				""Home Value""
				[
					""if Loan Purpose = Refi slash Cashout"" ""Mortgage Balance""
					""if Loan Purpose is not Refi slash Cashout`and (Loan Type = Cashout`OR Loan Purpose = Cashout Refi`OR Loan Purpose = Take Cash Out)"" ""Additional Funds""
					""if Loan Purpose is not Refi slash Cashout`and not (Loan Type = Cashout`OR Loan Purpose = Cashout Refi`OR Loan Purpose = Take Cash Out)"" ""Military""
				]
			

				""Mortgage Balance"" 
				[
					""Loan Type = Cashout`OR Loan Purpose = Cashout Refi`OR Loan Purpose = Take Cash Out"" ""Additional Funds""
					""not (Loan Type = Cashout`OR Loan Purpose = Cashout Refi`OR Loan Purpose = Take Cash Out)"" ""Military""
				]
			
				""Additional Funds""
			
				""Military""
				[
					""If Loan Type = Purchase"" ""Working w/Agent""
			
					""If Loan Type <> Purchase"" ""Credit Profile""
				]
			
				""Working w/Agent""
			
				""Purchase Price""
			
				""Down Payment""
			
				""Credit Profile""
				[
					""Loan Type = Refi slash C.O."" ""Second Mortgage""
			
					""Loan Type <> Refi slash C.O.`and QPV <> NoEmp"" ""Employment Status""
					""Loan Type <> Refi slash C.O.`and QPV = NoEmp`and Loan Type = Purchase`And F.T.H.B. = No"" ""Late Payments""
					""Loan Type <> Refi slash C.O.`and QPV = NoEmp`and NOT (Loan Type = Purchase`And F.T.H.B. = No)"" ""Bankruptcy""
				]
			
				""Second Mortgage""
				[
					""QPV <> NoEmp"" ""Employment Status""
					""Loan Type = Refi slash C.O.`and QPV = NoEmp`and Loan Type = Purchase`And F.T.H.B. = No"" ""Late Payments""
					""Loan Type = Refi slash C.O.`and QPV = NoEmp`and NOT (Loan Type = Purchase`And F.T.H.B. = No)"" ""Bankruptcy""
				]
			
				""Late Payments""
			
				""Foreclosure""
				[
					_ ""Bankruptcy""
				]
			
				""Employment Status""
			
				""Bankruptcy""
			
				""Name""
			
				""Home Phone""
			
				""Email""
			
				""Address and Zip"" anz
			
				action ""Submit Transaction"" a24
				[
					""Error"" anz
					""Successful"" redirect
				]
			
				""Redirect to slash thank-you"" redirect
			
				End
			]
";
		}
		public static string GetWorkItems()
		{
			return @"
Title:This is a sample project
StartDate:2022-08-31
3188738`""ALF[Onboarding] Nested Questions -Smart Form""`5
3188725`""ALF [Onboarding] Nested Question Set | UI""`5`3188738,2022 - 09 - 01
";
		}

		public static string GetC4Sample()
        {
			return @"
workspace 
[
    items
    [
        external_person customer ""Customer"" (""A customer of the bank,`with personal bank accounts"")

		enterprise e1 ""Big Co""
		[
			system c1 ""Internet Banking""
			[
				Container web_app ""Web Application"" utilizing ""Java, Spring MVC""(""Delivers the static content`and the Internet banking SPA"")

				Container backend_api ""API Application"" utilizing ""Java, Docker Container""(""Provides Internet banking`functionality via API"")

				Container spa ""Single-Page App"" utilizing ""JavaScript, Angular""(""Provides all the Internet banking`functionality to cutomers`via their web browser"")

				Container mobile_app ""Mobile App"" utilizing ""C#, Xamarin""(""Provides a limited subset`of the Internet banking`functionality to customers`via their mobile device"")

				Database database ""Database"" utilizing ""SQL Database""(""Stores user registration`information, hashed auth credentials,`access logs, etc."")
				[
					Table table1 ""Table 1""

					Table table2 ""Table 2""

					Table table3 ""Table 3""
				]
			]

			system banking_system ""Mainframe Banking System""(""Stores all of the core`banking information about`customers, accounts, transactions, etc."")
		]

		external_system email_system ""E-Mail System""(""The internal`Microsoft Exchange system"")
    ]
    
    connections
	[
		customer Uses web_app utilizing ""HTTPS""

		customer Uses spa utilizing ""HTTPS""

		customer Uses mobile_app

		web_app Delivers spa

		spa Uses backend_api utilizing ""async, JSON/HTTPS""

		mobile_app Uses backend_api utilizing ""async, JSON/HTTPS""

		database ""Reads from and writes to"" backend_api utilizing ""sync, JDBC""

		email_system ""Sends e-mails to"" customer

		backend_api ""Sends e-mails using"" email_system utilizing ""sync, SMTP""

		backend_api Uses banking_system utilizing ""sync/async, XML/HTTPS""
	]
]
";

		}
	}
}
