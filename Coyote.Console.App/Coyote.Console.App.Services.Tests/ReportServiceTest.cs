using System;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class ReportServiceTest
    {
        private Mock<IConfiguration> _mockIConfiguration;
        public ReportServiceTest()
        {
            _mockIConfiguration = new Mock<IConfiguration>();
        }

        #region Item Sales Report Test Cases
        [Fact]
        public void GetItemSalesReport_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetItemSales(reportRequest, ReportType.Department.ToString(), mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetItemSalesReport_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetItemSales(null, ReportType.Department.ToString(), null));
        }
        #endregion

        #region Sales and Stock Trx Sheet Test Cases
        [Fact]
        public void GetSalesAndStockTrxSheet_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new StockTrxSheetRequestModel
            {
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01")
            };
            var securityViewModel = new SecurityViewModel();

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetSalesAndStockTrxSheet(reportRequest, securityViewModel, ReportType.Sales.ToString());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetSalesAndStockTrxSheet_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);
            var securityViewModel = new SecurityViewModel();
            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetSalesAndStockTrxSheet(null, securityViewModel, ReportType.Sales.ToString()));
        }
        #endregion

        #region Journal Sales Financial Summary Test Cases
        [Fact]
        public void GetJournalSalesFinancialSummary_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new JournalSalesRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                StoreIds = "95",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetJournalSalesFinancialSummary(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetJournalSalesFinancialSummary_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetJournalSalesFinancialSummary(null, null));
        }

        [Fact]
        public void GetJournalSalesFinancialSummary_NullReference_Outlet_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new JournalSalesRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                StoreIds = null,
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetJournalSalesFinancialSummary(reportRequest, null));
        }
        #endregion

        #region Journal Sales Royalty And Advertising Summary Test Cases
        [Fact]
        public void GetJournalSalesRoyaltyAndAdvertisingSummary_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new JournalSalesRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                StoreIds = "95",
                DepartmentIds = "18",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetJournalSalesRoyaltyAndAdvertisingSummary(reportRequest, ReportType.Royalty.ToString(), mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetJournalSalesRoyaltyAndAdvertisingSummary_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetJournalSalesRoyaltyAndAdvertisingSummary(null, ReportType.Royalty.ToString(), null));
        }

        [Fact]
        public void GetJournalSalesRoyaltyAndAdvertisingSummary_NullReference_Outlet_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new JournalSalesRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                StoreIds = null,
                DepartmentIds = null,
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetJournalSalesRoyaltyAndAdvertisingSummary(reportRequest, ReportType.Royalty.ToString(), null));
        }
        #endregion

        #region Item Sales Summary Report Test Cases
        [Fact]
        public void GetItemSalesSummary_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new SalesSummaryRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                OrderByAmt = true,
                StoreIds = "95",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetItemSalesSummary(reportRequest, ReportType.Department.ToString(), mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetItemSalesSummary_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetItemSalesSummary(null, ReportType.Department.ToString(), null));
        }
        #endregion

        #region Item With No Sales Product Summary Report Test Cases
        [Fact]
        public void GetItemWithNoSalesProduct_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new NoSalesSummaryRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                StoreIds = "95",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetItemWithNoSalesProduct(reportRequest, ReportType.ItemNoSales.ToString(), mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetItemWithNoSalesProduct_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetItemWithNoSalesProduct(null, ReportType.ItemNoSales.ToString(), null));
        }
        #endregion

        #region Ranking By Outlet Test Cases
        [Fact]
        public void GetRankingByOutlet_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new RankingOutletRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetRankingByOutlet(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetRankingByOutlet_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetRankingByOutlet(null, null));
        }
        #endregion

        #region Stock Purchase Test Cases 
        [Fact]
        public void GetStockPurchase_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new StockPurchaseReportRequest
            {
                Format = "pdf",
                OrderInvoiceStartDate = Convert.ToDateTime("2020-04-01"),
                OrderInvoiceEndDate = Convert.ToDateTime("2020-04-01"),
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetStockPurchase(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetStockPurchase_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetStockPurchase(null, null));
        }
        #endregion

        #region Stock Variance Test Cases 
        [Fact]
        public void GetStockVariance_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetStockVariance(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetStockVariance_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetStockVariance(null, null));
        }
        #endregion

        #region Stock On Hand Test Cases 
        [Fact]
        public void GetStockOnHand_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetStockOnHand(reportRequest, mime,1);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetStockOnHand_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetStockVariance(null, null));
        }
        #endregion

        #region Stock Wastage Product Wise Test Cases 
        [Fact]
        public void GetStockWastageProductWise_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetStockWastageProductWise(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetStockWastageProductWise_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetStockWastageProductWise(null, null));
        }
        #endregion

        #region Stock Adjustment Test Cases 
        [Fact]
        public void GetStockAdjustment_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetStockAdjustment(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetStockAdjustment_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetStockAdjustment(null, null));
        }
        #endregion

        #region Cost Varience Test Cases 
        [Fact]
        public void CostVarience_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new CostVarianceFilters
            {
                Format = "pdf",
                StoreId = 95,
                InvoiceDateFrom = Convert.ToDateTime("2020-04-01"),
                InvoiceDateTo = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.CostVarience(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void CostVarience_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.CostVarience(null, null));
        }
        #endregion

        #region Sales Chart Test Cases
        [Fact]
        public void SalesChart_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            var securityViewModel = new SecurityViewModel();

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.SalesChart(reportRequest, ReportType.Department.ToString());

            Assert.NotNull(result);
        }

        [Fact]
        public void SalesChart_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);
            var securityViewModel = new SecurityViewModel();
            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.SalesChart(null, ReportType.Department.ToString()));
        }
        #endregion

        #region Sales Chart By Id Test Cases
        [Fact]
        public void SalesChartById_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StoreIds = "95",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            var securityViewModel = new SecurityViewModel();

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.SalesChartById(reportRequest, ReportType.Department.ToString());

            Assert.NotNull(result);
        }

        [Fact]
        public void SalesChartById_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);
            var securityViewModel = new SecurityViewModel();
            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.SalesChartById(null, ReportType.Department.ToString()));
        }
        #endregion

        #region Sales Chart Detailed Test Cases
        [Fact]
        public void SalesChartDetailed_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new ReportRequestModel
            {
                Format = "pdf",
                StoreIds = "95",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            var securityViewModel = new SecurityViewModel();

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.SalesChartDetailed(reportRequest, ReportType.Department.ToString());

            Assert.NotNull(result);
        }

        [Fact]
        public void SalesChartDetailed_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);
            var securityViewModel = new SecurityViewModel();
            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.SalesChartDetailed(null, ReportType.Department.ToString()));
        }
        #endregion

        #region Report Filteration Dropdown List Test Cases
        [Fact]
        public void ReportFilterationDropdownList_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var securityViewModel = new SecurityViewModel();
            var pagedInput = new PagedInputModel
            {
                MaxResultCount = 5,
            };

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.ReportFilterationDropdownList(securityViewModel, pagedInput);

            Assert.NotNull(result);
        }
        #endregion

        #region Item Purchase Summary Report Test Cases
        [Fact]
        public void GetItemPurchaseSummary_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new SalesSummaryRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                OrderByAmt = true,
                StoreIds = "95",
                DepartmentIds = "16,17",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetItemPurchaseSummary(reportRequest, ReportType.Department.ToString(), mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetItemPurchaseSummary_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetItemSalesSummary(null, ReportType.Department.ToString(), null));
        }
        #endregion

        #region Ranging Report Test Cases
        [Fact]
        public void GetRanging_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new RangingRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-02"),
                StoreIds = "95",
                DepartmentIds="16,17,18",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetRanging(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetRanging_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetRanging(null, null));
        }
        #endregion

        #region National Ranging Report Test Cases
        [Fact]
        public void GetNationalRanging_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new NationalRangingRequestModel
            {
                Format = "pdf",
                StoreIds = "95",
                DepartmentIds="16,17",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetNationalRanging(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetNationalRanging_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetNationalRanging(null, null));
        }
        #endregion

        #region Product Price Deviation Report Test Cases
        [Fact]
        public void GetProductPriceDeviation_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new NationalRangingRequestModel
            {
                Format = "pdf",
                StoreIds = "95",
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
          //  var result = mockReportService.GetProductPriceDeviation(reportRequest, mime);

           // Assert.NotNull(result);
        }

        [Fact]
        public void GetProductPriceDeviation_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
          //  Assert.Throws<NullReferenceException>(() => mockReportService.GetProductPriceDeviation(null, null));
        }
        #endregion

        #region National Level Sales Summary Report Test Cases
        [Fact]
        public void GetNationalLevelSalesSummary_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new NationalLevelRequestModel
            {
                Format = "pdf",
                StoreIds = "95",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetNationalLevelSalesSummary(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetNationalLevelSalesSummary_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetNationalLevelSalesSummary(null, null));
        }
        #endregion

        #region KPI Ranking Test Cases
        [Fact]
        public void GetKPIRanking_Valid_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            var reportRequest = new KPIRankingRequestModel
            {
                Format = "pdf",
                StartDate = Convert.ToDateTime("2020-04-01"),
                EndDate = Convert.ToDateTime("2020-04-01"),
                Inline = false
            };
            string mime = "application/" + reportRequest.Format;

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            var result = mockReportService.GetKPIRanking(reportRequest, mime);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetKPIRanking_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetKPIRanking(null, null));
        }
        #endregion

        #region Items With Hourly Sales Test Cases
        //[Fact]
        //public void GetItemsWithHourlySales_Valid_Test()
        //{
        //    //Arrange
        //    var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        //    _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
        //    _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

        //    var reportRequest = new ReportRequestModel
        //    {
        //        Format = "pdf",
        //        StartDate = Convert.ToDateTime("2020-04-01"),
        //        EndDate = Convert.ToDateTime("2020-04-01"),
        //        Inline = false
        //    };
        //    string mime = "application/" + reportRequest.Format;

        //    // Act 
        //    var mockReportService = new ReportService(_mockIConfiguration.Object);

        //    // Assert
        //    var result = mockReportService.GetItemsWithHourlySales(reportRequest, ReportType.Commodity.ToString(), mime);

        //    Assert.NotNull(result);
        //}

        [Fact]
        public void GetItemsWithHourlySales_NullReference_Test()
        {
            //Arrange
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "FastReportSettings:ReportPath")]).Returns(config["FastReportSettings:ReportPath"]);
            _mockIConfiguration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DBConnection")]).Returns(config["ConnectionStrings:DBConnection"]);

            // Act 
            var mockReportService = new ReportService(_mockIConfiguration.Object);

            // Assert
            Assert.Throws<NullReferenceException>(() => mockReportService.GetItemsWithHourlySales(null, ReportType.Commodity.ToString(), null));
        }
        #endregion
    }
}
