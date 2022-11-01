using System;
using Coyote.Console.Common;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.Tests.MockRepository;
using Coyote.Console.ViewModels.ViewModels;
using NUnit.Framework;

namespace Coyote.Console.ViewModels.Tests
{
    [TestFixture]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "NUnit Test Class")]
    public class ViewModelTests
    {
        #region ResetUser model

        [Test]
        public void ResetUserRequestModelValidData()
        {
            //Arrange
            var resetUser = new ResetPasswordViewModel
            {
                NewPassword = "Abcd@1234",
                TempPassword = Guid.NewGuid().ToString(),
                UserId = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(resetUser).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("", TestName = "New Password As Blank")]
        [TestCase(" ", TestName = "New Password As Blank Spaces")]
        [TestCase("UserPassword", TestName = "Invalid Password")]
        public void ResetUserRequestModelNewPasswordInValid(string password)
        {
            //Arrange
            var resetUser = new ResetPasswordViewModel
            {
                NewPassword = password,
                TempPassword = Guid.NewGuid().ToString(),
                UserId = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(resetUser).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }



        [Test]
        [TestCase("", TestName = "Temp Password As Blank")]
        [TestCase(" ", TestName = "Temp Password As Blank Spaces")]
        public void ResetUserRequestModelTempPasswordInValid(string tempPasword)
        {
            //Arrange
            var resetUser = new ResetPasswordViewModel
            {
                NewPassword = "Abcd@1234",
                TempPassword = tempPasword,
                UserId = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(resetUser).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }



        [Test]
        public void ResetUserRequestModelNullPasswordInValid()
        {
            //Arrange
            var resetUser = new ResetPasswordViewModel
            {
                NewPassword = null,
                TempPassword = null,
                UserId = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(resetUser).Count;

            //Assert
            Assert.AreEqual(2, errorcount);
        }


        #endregion

        #region LoginViewModel
        [Test]
        public void LoginRequestModelValidData()
        {
            //Arrange
            var loginUser = new LoginViewModel
            {
                UserEmail = "uses@email.com"
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(loginUser).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Password As Blank")]
        [TestCase(" ", TestName = "Password As Blank Spaces")]
        [TestCase("UserPassword", TestName = "Invalid Password")]
        public void LoginRequestModelPasswordInValid()
        {
            //Arrange
            var loginUser = new LoginViewModel
            {
                UserEmail = "uses@email.com"
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(loginUser).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Email As Blank")]
        [TestCase(" ", TestName = "Email As Blank Spaces")]
        public void LoginRequestModelUserEmailInValid(string email)
        {
            //Arrange
            var loginUser = new LoginViewModel
            {
                UserEmail = email
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(loginUser).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }


        [Test]
        public void LoginRequestModelEmailPasswordNullInValid()
        {
            //Arrange
            var loginUser = new LoginViewModel
            {
                UserEmail = null
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(loginUser).Count;

            //Assert
            Assert.AreEqual(2, errorcount);
        }


        #endregion

        #region UserRole
        [Test]
        public void UserRoleValidData()
        {
            //Arrange
            var userRole = new UserRoleViewModel
            {
                //Code = "ValidRoleCode",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedById = 1,
                UpdatedById = 1,
                Id = 1,
                IsDefault = true,
                //Name = "Admin",
                RoleId = 1,
                //StoreId = 1,
                //Type = "Admin",
                UserId = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(userRole).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        //[TestCase("UserROleTest123{Random Name}", TestName = "Name As Combination Of Numbers And Characters")]
        //[TestCase("RoleNameRoleNameRoleNameRoleNameRoleNameRoleName", TestName = "Name Upto 50 characters")]
        //[TestCase("P", TestName = "Name as 1 character")]
        //public void UserRoleValidName(string name)
        //{
        //    //Arrange
        //    var userRole = new UserRoleViewModel
        //    {
        //        //Code = "Cd123",
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now,
        //        CreatedById = 1,
        //        UpdatedById = 1,
        //        Id = 1,
        //        IsDefault = true,
        //        //Name = name,
        //        RoleId = 1,
        //        StoreId = 1,
        //        //Type = "Admin",
        //        UserId = 1
        //    };
        //    //Act
        //    var errorcount = MockViewModelRepository.ValidateModel(userRole).Count;

        //    //Assert
        //    Assert.AreEqual(0, errorcount);
        //}

        //[TestCase("", TestName = "Name As Blank")]
        //[TestCase(" ", TestName = "Name As Blank Spaces")]
        //[TestCase("RoleNameRoleNameRoleNameRoleNameRoleNameRoleNameRoleName", TestName = "Name Upto 50 characters")]
        //public void UserRoleNameInValid(string name)
        //{
        //    //Arrange
        //    var userRole = new UserRoleViewModel
        //    {
        //        //Code = "123",
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now,
        //        CreatedById = 1,
        //        UpdatedById = 1,
        //        Id = 1,
        //        IsDefault = true,
        //        //Name = name,
        //        RoleId = 1,
        //        StoreId = 1,
        //        //Type = "Admin",
        //        UserId = 1
        //    };

        //    //Act
        //    var errorcount = MockViewModelRepository.ValidateModel(userRole).Count;

        //    //Assert
        //    Assert.AreEqual(1, errorcount);
        //}
        #endregion

        #region Role View Model

        [Test]
        public void RoleModelValidData()
        {
            //Arrange
            var roleModel = new RolesViewModel
            {
                Id = 0,
                Code = "Admin",
                Name = "Administrator",
                Type = "Admin",
                Status = true,
                PermissionSet = "[Read,Write]"
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(roleModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);


        }


        [Test]
        public void RoleCodeMaxLength()
        {
            //Arrange
            var roleModel = new RolesViewModel
            {
                Id = 0,
                Code = "Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin ",
                Name = "Administrator",
                Type = "Admin",
                Status = true,
                PermissionSet = "[Read,Write]"
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(roleModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.RoleCode, error[0].ErrorMessage);


        }

        [Test]
        public void RoleNameMaxLength()
        {
            //Arrange
            var roleModel = new RolesViewModel
            {
                Id = 0,
                Code = "Admin",
                Name = "Administrator Administrator Administrator Administrator Administrator Administrator Administrator Administrator Administrator Administrator Administrator ",
                Type = "Admin",
                Status = true,
                PermissionSet = "[Read,Write]"
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(roleModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.RoleName, error[0].ErrorMessage);

        }

        [Test]
        public void RoleTypeMaxLength()
        {
            //Arrange
            var roleModel = new RolesViewModel
            {
                Id = 0,
                Code = "Admin",
                Name = "Administrator",
                Type = "Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin Admin ",
                Status = true,
                PermissionSet = "[Read,Write]"
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(roleModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.RoleType, error[0].ErrorMessage);

        }

        #endregion

        #region Stock Adjust View Model

        [Test]
        public void StockAdjustValidData()
        {
            //Arrange 
            var header = new StockAdjustHeaderRequestModel
            {
                OutletId = 101,
                Reference = "10101"
            };

            //Act
            var errorCount = MockViewModelRepository.ModelValidationCheck(header).Count;

            //Assert
            Assert.AreEqual(0, errorCount);
        }

        [Test]
        public void StockAdjustHeaderReferenceMaxLength()
        {
            //Arrange
            var header = new StockAdjustHeaderRequestModel
            {
                OutletId = 101,
                Reference = "10101 10101 10101"
            };

            //ACT
            var error = MockViewModelRepository.ValidateModel(header);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.StockAdjustLength, error[0].ErrorMessage);
        }
        #endregion

        #region Order View Model
        [Test]
        public void OrderValidData()
        {
            //Arrange
            var order = new OrderRequestModel
            {
                Reference = "Test Refer",
                DeliveryNo = "Test",
                InvoiceNo = "Test",

            };

            //ACT
            var errorCount = MockViewModelRepository.ModelValidationCheck(order).Count;

            //Assert
            Assert.AreEqual(0, errorCount);
        }

        [Test]
        public void OrderReferenceMaxLength()
        {
            //Arrange
            var order = new OrderRequestModel
            {
                OutletId = 101,
              //  OrderNo = 101,
                Reference = "Test Refer Test Refer Test Refer Test Refer Test Refer Test Refer",
                DeliveryNo = "Test",
                InvoiceNo = "Test",
                CreatedDate = DateTime.UtcNow.AddDays(-3),
            };

            //ACT
            var error = MockViewModelRepository.ValidateModel(order);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.OrderRefLength, error[0].ErrorMessage);
        }
        [Test]
        public void OrderDeliveryNoMaxLength()
        {
            //Arrange
            var order = new OrderRequestModel
            {
                Reference = "Test Refer",
                DeliveryNo = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test ",
                InvoiceNo = "Test",

            };

            //ACT
            var error = MockViewModelRepository.ValidateModel(order);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.OrderDeliveryNoLength, error[0].ErrorMessage);
        }
        [Test]
        public void OrderInvoiceNoMaxLength()
        {
            //Arrange
            var order = new OrderRequestModel
            {
                Reference = "Test Refer",
                DeliveryNo = "Test",
                InvoiceNo = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test ",

            };

            //ACT
            var error = MockViewModelRepository.ValidateModel(order);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.OrderInvoiceNoLength, error[0].ErrorMessage);
        }
        [Test]
        public void OrderDetailBuyPromoSupplierMaxLength()
        {
            //Arrange
            var order =
                   new OrderDetailRequestModel
                   {
                       BuyPromoCode = "Test Test Test Test Test Test Test Test Test Test ",
                       SalePromoCode = "Test",

                   };

            //ACT
            var error = MockViewModelRepository.ValidateModel(order);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.OrderBuyPromoCodeLength, error[0].ErrorMessage);
        }
        [Test]
        public void OrderDetailSalePromoSupplierMaxLength()
        {
            //Arrange
            var orderDetail =
                   new OrderDetailRequestModel
                   {
                       BuyPromoCode = "Test",
                       SalePromoCode = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test ",

                   };

            //ACT
            var error = MockViewModelRepository.ValidateModel(orderDetail);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.OrderSalePromoCodeLength, error[0].ErrorMessage);
        }
        #endregion

        #region Promotion View Model

        [Test]
        public void PromotionValidData()
        {
            //Arrange
            var promotion = new PromotionRequestModel
            {
                Code = "Promo-1",
                Desc = "Description of Promotion",
                RptGroup = "Report Group",
                Start = DateTime.UtcNow.AddDays(-3),
                End = DateTime.UtcNow.AddDays(10),
                Availibility = "Availability"
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(promotion).Count;

            //Assert
            Assert.AreEqual(0, errorcount);

        }

        [Test]
        public void PromotionPromotionCodeMaxLength()
        {
            //Arrange
            var promotion = new PromotionRequestModel
            {
                Code = "Promo-1 Promo-1 Promo-1",
                Desc = "Description of Promotion",
                RptGroup = "Report Group",
                Start = DateTime.UtcNow.AddDays(-3),
                End = DateTime.UtcNow.AddDays(10),
                Availibility = "Availability"
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionCodeLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionPromotionDescMaxLength()
        {
            //Arrange
            var promotion = new PromotionRequestModel
            {
                Code = "Promo-1",
                Desc = "Description of Promotion Description of Promotion Description of Promotion Description of Promotion",
                RptGroup = "Report Group",
                Start = DateTime.UtcNow.AddDays(-3),
                End = DateTime.UtcNow.AddDays(10),
                Availibility = "Availability"
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionDescLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionPromotionRptGroupMaxLength()
        {
            //Arrange
            var promotion = new PromotionRequestModel
            {
                Code = "Promo-1",
                Desc = "Description of Promotion",
                RptGroup = "Report Group Report Group Report Group",
                Start = DateTime.UtcNow.AddDays(-3),
                End = DateTime.UtcNow.AddDays(10),
                Availibility = "Availability"
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionRptGroupLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionPromotionAvailibilityMaxLength()
        {
            //Arrange
            var promotion = new PromotionRequestModel
            {
                Code = "Promo-1",
                Desc = "Description of Promotion",
                RptGroup = "Report Group",
                Start = DateTime.UtcNow.AddDays(-3),
                End = DateTime.UtcNow.AddDays(10),
                Availibility = "Availability Availability Availability Availability Availability Availability Availability Availability Availability Availability " +
                "Availability Availability Availability Availability Availability Availability Availability Availability Availability Availability " +
                "Availability Availability Availability Availability Availability Availability Availability Availability Availability Availability " +
                "Availability Availability Availability Availability Availability Availability Availability Availability Availability Availability "
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionAvailibilityLength, error[0].ErrorMessage);
        }

        #region Buying
        [Test]
        public void PromotionBuyingValidData()
        {
            //Arrange
            var promotion = new PromotionBuyingViewModel
            {
                Desc = "Promotion Buying Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(promotion).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        public void PromotionBuyingDescMaxLength()
        {
            //Arrange
            var promotion = new PromotionBuyingViewModel
            {
                Desc = "Promotion Buying Description Promotion Buying Description Promotion Buying Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductDescLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionBuyingActionMaxLength()
        {
            //Arrange
            var promotion = new PromotionBuyingViewModel
            {
                Desc = "Promotion Buying Description",
                Action = "Test Test Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductActionLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionBuyingHostPromoTypeMaxLength()
        {
            //Arrange
            var promotion = new PromotionBuyingViewModel
            {
                Desc = "Promotion Buying Description",
                Action = "Test",
                HostPromoType = "Test Test Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductHostPromoTypeLength, error[0].ErrorMessage);
        }

        #endregion

        #region Selling
        [Test]
        public void PromotionSellingValidData()
        {
            //Arrange
            var promotion = new PromotionSellingViewModel
            {
                Desc = "Promotion Selling Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(promotion).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        public void PromotionSellingDescMaxLength()
        {
            //Arrange
            var promotion = new PromotionSellingViewModel
            {
                Desc = "Promotion Selling Description Promotion Selling Description Promotion Selling Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductDescLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionSellingActionMaxLength()
        {
            //Arrange
            var promotion = new PromotionSellingViewModel
            {
                Desc = "Promotion Selling Description",
                Action = "Test Test Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductActionLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionSellingHostPromoTypeMaxLength()
        {
            //Arrange
            var promotion = new PromotionSellingViewModel
            {
                Desc = "Promotion Selling Description",
                Action = "Test",
                HostPromoType = "Test Test Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductHostPromoTypeLength, error[0].ErrorMessage);
        }

        #endregion

        #region Member offer

        [Test]
        public void PromotionMemberOfferValidData()
        {
            //Arrange
            var promotion = new PromotionMemberOfferViewModel
            {
                Desc = "Promotion Member Offer Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(promotion).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        public void PromotionMemberOfferDescMaxLength()
        {
            //Arrange
            var promotion = new PromotionMemberOfferViewModel
            {
                Desc = "Promotion Member Offer Description Promotion Member Offer Description Promotion Member Offer Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductDescLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionMemberOfferActionMaxLength()
        {
            //Arrange
            var promotion = new PromotionMemberOfferViewModel
            {
                Desc = "Promotion Member Offer Description",
                Action = "Test Test Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductActionLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionMemberOfferHostPromoTypeMaxLength()
        {
            //Arrange
            var promotion = new PromotionMemberOfferViewModel
            {
                Desc = "Promotion Member Offer Description",
                Action = "Test",
                HostPromoType = "Test Test Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductHostPromoTypeLength, error[0].ErrorMessage);

        }
        #endregion


        #region Member offer

        [Test]
        public void PromotionOfferValidData()
        {
            //Arrange
            var promotion = new PromotionOfferProductViewModel
            {
                Desc = "Promotion Offer Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(promotion).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        public void PromotionOfferDescMaxLength()
        {
            //Arrange
            var promotion = new PromotionOfferProductViewModel
            {
                Desc = "Promotion Offer Description Promotion Offer Description Promotion Offer Description Promotion Offer Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductDescLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionOfferActionMaxLength()
        {
            //Arrange
            var promotion = new PromotionOfferProductViewModel
            {
                Desc = "Promotion Offer Description",
                Action = "Test Test Test Test Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductActionLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionOfferHostPromoTypeMaxLength()
        {
            //Arrange
            var promotion = new PromotionOfferProductViewModel
            {
                Desc = "Promotion Offer Description",
                Action = "Test",
                HostPromoType = "Test Test Test Test Test",
            };


            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductHostPromoTypeLength, error[0].ErrorMessage);

        }
        #endregion


        #region Mix Match offer

        [Test]
        public void PromotionMixMatchValidData()
        {
            //Arrange
            var promotion = new PromotionProductRequestModel
            {
                Desc = "Promotion mix match Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(promotion).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        public void PromotionMixMatchDescMaxLength()
        {
            //Arrange
            var promotion = new PromotionProductRequestModel
            {
                Desc = "Promotion mix match Description Promotion mix match Description Promotion mix match Description Promotion mix match Description",
                Action = "Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductDescLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionMixMatchActionMaxLength()
        {
            //Arrange
            var promotion = new PromotionProductRequestModel
            {
                Desc = "Promotion mix match Description",
                Action = "Test Test Test Test Test",
                HostPromoType = "Test",
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductActionLength, error[0].ErrorMessage);
        }

        [Test]
        public void PromotionMixMatchHostPromoTypeMaxLength()
        {
            //Arrange
            var promotion = new PromotionProductRequestModel
            {
                Desc = "Promotion mix match Description",
                Action = "Test",
                HostPromoType = "Test Test Test Test Test",
            };


            //Act
            var error = MockViewModelRepository.ValidateModel(promotion);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.PromotionProductHostPromoTypeLength, error[0].ErrorMessage);

        }
        #endregion

        #endregion


        #region Store Group View Model

        [Test]
        public void StoreGroupValidData()
        {
            //Arrange
            var storeGroupModel = new StoreGroupViewModel
            {
                Id = 0,
                Code = "SG-1",
                Name = "Store Group",
                AddedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true,
                CreatedById = 0,
                UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(storeGroupModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);


        }

        [Test]
        public void StoreGroupStoreGroupCodeMaxLength()
        {
            //Arrange
            var storeGroupModel = new StoreGroupViewModel
            {
                Id = 0,
                Code = "SG-1 SG-1 SG-1 SG-1 SG-1 SG-1 SG-1 SG-1 SG-1 SG-1 SG-1 ",
                Name = "Store Group",
                AddedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true,
                CreatedById = 0,
                UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(storeGroupModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.StoreGroupCode, error[0].ErrorMessage);

        }

        [Test]
        public void StoreGroupStoreGroupNameMaxLength()
        {
            //Arrange
            var storeGroupModel = new StoreGroupViewModel
            {
                Id = 0,
                Code = "SG-1",
                Name = "Store Group Store Group Store Group Store Group Store Group Store Group Store Group Store Group Store Group Store Group Store Group Store Group ",
                AddedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true,
                CreatedById = 0,
                UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(storeGroupModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.StoreGroupName, error[0].ErrorMessage);

        }


        #endregion

        #region Store View Model

        [Test]
        public void StoreViewModelValidate()
        {
            //Arrange
            var storeModel = new StoreRequestModel
            {
                Code = "CODE-STORE",
                GroupId = 0,
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                PhoneNumber = "00998877665544",
                Fax = "1234567890",
                PostCode = "1234",
                Status = true,
                Desc = "Description of Store",
                PriceZoneId = 12,
                SellingInd = false,
                StockInd = false,
                DelName = "Store Name",
                DelAddr1 = "Address-1",
                DelAddr2 = "Address-2",
                DelAddr3 = "Address-3",
                DelPostCode = "1234",
                CostType = "C",
                Abn = "S",
                FuelSite = true,
                BudgetGrowthFact = 0.00,
                CostZoneId = 12,
                EntityNumber = "000",
                LabelTypeShelfId = 1,
                LabelTypePromoId = 1,
                LabelTypeShortId = 1,
                OutletPriceFromOutletId = 00,
                PriceFromLevel = 30,
                WarehouseId = 00
            };
            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(storeModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        public void StoreStoreAddressMaxLength()
        {
            //Arrange
            var storeModel = new StoreRequestModel
            {

                Code = "CODE-STORE",
                GroupId = 0,
                Address1 = "Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 ",
                Address2 = "Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 ",
                Address3 = "Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 ",
                PhoneNumber = "00998877665544",
                Fax = "1234567890",
                PostCode = "1234",
                Status = true,
                Desc = "Description of Store",
                PriceZoneId = 12,
                SellingInd = false,
                StockInd = false,
                DelName = "Store Name",
                DelAddr1 = "Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 ",
                DelAddr2 = "Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 ",
                DelAddr3 = "Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 ",
                DelPostCode = "1234",
                CostType = "C",
                Abn = "S",
                FuelSite = true,
                BudgetGrowthFact = 0.00,
                CostZoneId = 12,
                EntityNumber = "000",
                LabelTypeShelfId = 1,
                LabelTypePromoId = 1,
                LabelTypeShortId = 1,
                OutletPriceFromOutletId = 00,
                PriceFromLevel = 30,
                WarehouseId = 00
            };
            //Act
            var errorcount = MockViewModelRepository.ValidateModel(storeModel).Count;

            //Assert
            Assert.AreEqual(6, errorcount);
        }

        [Test]
        public void StoreNumberMaxLength()
        {
            //Arrange
            var storeModel = new StoreRequestModel
            {

                Code = "CODE-STORE",
                GroupId = 0,
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                PhoneNumber = "1234567890 123456",
                Fax = "1234567890 123456",
                PostCode = "1234",
                Status = true,
                Desc = "Description of Store",
                PriceZoneId = 12,
                SellingInd = false,
                StockInd = false,
                DelName = "Store Name",
                DelAddr1 = "Address-1",
                DelAddr2 = "Address-2",
                DelAddr3 = "Address-3",
                DelPostCode = "1234",
                CostType = "C",
                Abn = "S",
                FuelSite = true,
                BudgetGrowthFact = 0.00,
                CostZoneId = 12,
                EntityNumber = "000",
                LabelTypeShelfId = 1,
                LabelTypePromoId = 1,
                LabelTypeShortId = 1,
                OutletPriceFromOutletId = 00,
                PriceFromLevel = 30,
                WarehouseId = 00
            };
            //Act
            var errorcount = MockViewModelRepository.ValidateModel(storeModel).Count;

            //Assert
            Assert.AreEqual(2, errorcount);
        }

        [Test]
        public void StorePostCodeMaxLength()
        {
            //Arrange
            var storeModel = new StoreRequestModel
            {
                Code = "CODE-STORE",
                GroupId = 0,
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                PhoneNumber = "1234567890",
                Fax = "1234567890",
                PostCode = "0000000",
                Status = true,
                Desc = "Description of Store",
                PriceZoneId = 12,
                SellingInd = false,
                StockInd = false,
                DelName = "Store Name",
                DelAddr1 = "Address-1",
                DelAddr2 = "Address-2",
                DelAddr3 = "Address-3",
                DelPostCode = "0000000",
                CostType = "C",
                Abn = "S",
                FuelSite = true,
                BudgetGrowthFact = 0.00,
                CostZoneId = 12,
                EntityNumber = "000",
                LabelTypeShelfId = 1,
                LabelTypePromoId = 1,
                LabelTypeShortId = 1,
                OutletPriceFromOutletId = 00,
                PriceFromLevel = 30,
                WarehouseId = 00
            };
            //Act
            var errorcount = MockViewModelRepository.ValidateModel(storeModel).Count;

            //Assert
            Assert.AreEqual(2, errorcount);
        }

        [Test]
        public void StoreStoreDecriptionMaxLength()
        {
            //Arrange
            var storeModel = new StoreRequestModel
            {
                Code = "CODE-STORE",
                GroupId = 0,
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                PhoneNumber = "1234567890",
                Fax = "1234567890",
                PostCode = "1234",
                Status = true,
                Desc = "Description of Store Description of Store Description of Store Description of Store Description of Store Description of Store",
                PriceZoneId = 12,
                SellingInd = false,
                StockInd = false,
                DelName = "Store Name",
                DelAddr1 = "Address-1",
                DelAddr2 = "Address-2",
                DelAddr3 = "Address-3",
                DelPostCode = "1234",
                CostType = "C",
                Abn = "S",
                FuelSite = true,
                BudgetGrowthFact = 0.00,
                CostZoneId = 12,
                EntityNumber = "000",
                LabelTypeShelfId = 1,
                LabelTypePromoId = 1,
                LabelTypeShortId = 1,
                OutletPriceFromOutletId = 00,
                PriceFromLevel = 30,
                WarehouseId = 00
            };
            //Act
            var errorcount = MockViewModelRepository.ValidateModel(storeModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        public void StoreStoreAbnMaxLength()
        {
            //Arrange
            var storeModel = new StoreRequestModel
            {
                Code = "CODE-STORE",
                GroupId = 0,
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                PhoneNumber = "1234567890",
                Fax = "1234567890",
                PostCode = "1234",
                Status = true,
                Desc = "Description of Store",
                PriceZoneId = 12,
                SellingInd = false,
                StockInd = false,
                DelName = "Store Name",
                DelAddr1 = "Address-1",
                DelAddr2 = "Address-2",
                DelAddr3 = "Address-3",
                DelPostCode = "1234",
                CostType = "C",
                Abn = "000000-000000-000000-000000-000000-000000-000000-000000-000000",
                FuelSite = true,
                BudgetGrowthFact = 0.00,
                CostZoneId = 12,
                EntityNumber = "000",
                LabelTypeShelfId = 1,
                LabelTypePromoId = 1,
                LabelTypeShortId = 1,
                OutletPriceFromOutletId = 00,
                PriceFromLevel = 30,
                WarehouseId = 00
            };
            //Act
            var errorcount = MockViewModelRepository.ValidateModel(storeModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        public void StoreEntityNumMaxLength()
        {
            //Arrange
            var storeModel = new StoreRequestModel
            {
                Code = "CODE-STORE",
                GroupId = 0,
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                PhoneNumber = "00998877665544",
                Fax = "1234567890",
                PostCode = "1234",
                Status = true,
                Desc = "Description of Store",
                PriceZoneId = 12,
                SellingInd = false,
                StockInd = false,
                DelName = "Store Name",
                DelAddr1 = "Address-1",
                DelAddr2 = "Address-2",
                DelAddr3 = "Address-3",
                DelPostCode = "1234",
                CostType = "C",
                Abn = "S",
                FuelSite = true,
                BudgetGrowthFact = 0.00,
                CostZoneId = 12,
                EntityNumber = "000-000-000",
                LabelTypeShelfId = 1,
                LabelTypePromoId = 1,
                LabelTypeShortId = 1,
                OutletPriceFromOutletId = 00,
                PriceFromLevel = 30,
                WarehouseId = 00
            };
            //Act
            var errorcount = MockViewModelRepository.ValidateModel(storeModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }
        #endregion

        #region Supplier

        [Test]
        public void SupplierViewModelValidate()
        {
            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };
            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(supplierModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }


        [Test]
        public void SupplierCodeMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code Supplier Code Supplier Code Supplier Code Supplier Code Supplier Code Supplier Code Supplier Code ",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppCode, error[0].ErrorMessage);
        }

        [Test]
        public void SupplierAddressMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 Address-1 ",
                Address2 = "Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 Address-2 ",
                Address3 = "Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 Address-3 ",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(3, error.Count);
            Assert.AreEqual(ErrorMessages.SuppAddress1, error[0].ErrorMessage);
            Assert.AreEqual(ErrorMessages.SuppAddress2, error[1].ErrorMessage);
            Assert.AreEqual(ErrorMessages.SuppAddress3, error[2].ErrorMessage);
        }

        [Test]
        public void SupplierPhoneFaxMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "000000000000000000",
                Fax = "000000000000000000",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(2, error.Count);
            Assert.AreEqual(ErrorMessages.SuppPhone, error[0].ErrorMessage);
            Assert.AreEqual(ErrorMessages.SuppFax, error[1].ErrorMessage);
        }

        [Test]
        public void SupplierEmailMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier_coyote_Supplier_coyote@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppEmail, error[0].ErrorMessage);
        }

        [Test]
        public void SupplierAbnMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "1234567890-123456",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppABN, error[0].ErrorMessage);
        }


        [Test]
        public void SupplierUpdateCostMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "00000",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppUpdateCost, error[0].ErrorMessage);
        }

        [Test]
        public void SupplierPromoSupplierMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 Promo#101 ",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppPromoCode, error[0].ErrorMessage);
        }


        [Test]
        public void SupplierContactMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier Suppllier ",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppContactName, error[0].ErrorMessage);
        }

        [Test]
        public void SupplierCostZoneMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone Cost Zone ",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppCostZone, error[0].ErrorMessage);
        }

        [Test]
        public void SupplierItemCodeMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code ",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code Item Code ",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(2, error.Count);
            Assert.AreEqual(ErrorMessages.SuppGSTFreeItemCode, error[0].ErrorMessage);
            Assert.AreEqual(ErrorMessages.SuppGSTInclItemCode, error[1].ErrorMessage);
        }

        [Test]
        public void SupplierItemDescMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description Item Description Item Description Item Description Item Description Item Description Item Description Item Description Item Description ",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description Item Description Item Description Item Description Item Description Item Description Item Description Item Description Item Description ",
                XeroName = "Xero",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(2, error.Count);
            Assert.AreEqual(ErrorMessages.SuppGSTFreeItemDesc, error[0].ErrorMessage);
            Assert.AreEqual(ErrorMessages.SuppGSTInclItemDesc, error[1].ErrorMessage);

        }
        [Test]
        public void SupplierXeroLengthMaxlength()
        {

            //Arrange
            var supplierModel = new SupplierRequestModel
            {
                //Id = 0,
                Code = "Supplier Code",
                Desc = "Description of Supplier",
                Address1 = "Address-1",
                Address2 = "Address-2",
                Address3 = "Address-3",
                Phone = "009988776655",
                Fax = "009988776655",
                Email = "supplier@coyote.com",
                ABN = "123",
                UpdateCost = "100",
                PromoSupplier = "101",
                Contact = "Suppllier",
                CostZone = "Cost Zone",
                GSTFreeItemCode = "Item Code",
                GSTFreeItemDesc = "Item Description",
                GSTInclItemCode = "Item Code",
                GSTInclItemDesc = "Item Description",
                XeroName = "Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero Xero ",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(supplierModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.SuppXeroName, error[0].ErrorMessage);
        }

        #endregion

        #region Zone Outlet


        [Test]
        public void ZoneOutletModelValidData()
        {
            //Arrange
            var zoneOUtletModel = new ZoneOutletRequestModel
            {
                StoreIds = "1,2,3",
                ZoneId = 0,

            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(zoneOUtletModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        #endregion

        #region DepartmentViewModel

        [Test]
        public void DepartmentValidData()
        {
            //Arrange
            var dept = new DepartmentRequestModel
            {
                //Id = 1,
                Code = "Dept-121",
                Desc = "Description about the Department",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 1,
                //UpdatedById = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(dept).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Department Code is blank")]
        [TestCase(" ", TestName = "Department Code is blank spaces")]
        [TestCase("Dept_CodeDept_CodeDept_CodeDept_CodeDept_CodeDept_Code", TestName = "Name Upto 30 characters")]

        public void DepartmentCodeInValid(string code)
        {
            //Arrange
            var dept = new DepartmentRequestModel
            {
                //Id = 1,
                Code = code,
                Desc = "Description about the Department",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 1,
                //UpdatedById = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(dept).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Department Desc is blank")]
        [TestCase(" ", TestName = "Department Desc is blank spaces")]
        [TestCase("Dept_DescDept_DescDept_DescDept_DescDept_DescDept_DescDept_DescDept_DescDept_Desc", TestName = "Desc Upto 80 characters")]

        public void DepartmentDescInValid(string desc)
        {
            //Arrange
            var dept = new DepartmentRequestModel
            {
                //Id = 1,
                Code = "ValidCode",
                Desc = desc,
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 1,
                //UpdatedById = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(dept).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [TestCase("DepartmentTest123{Random Desc}", TestName = "Desc As Combination Of Numbers And Characters")]
        [TestCase("DepartmentDescription_DepartmentDescription", TestName = "Desc Upto 80 characters")]
        [TestCase("D", TestName = "Description as 1 character")]
        public void DepartmentDescValid(string desc)
        {
            //Arrange
            var dept = new DepartmentRequestModel
            {
                //Id = 1,
                Code = "ValidCode",
                Desc = desc,
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 1,
                //UpdatedById = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(dept).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [TestCase("Department123{Random Code}", TestName = "Code As Combination Of Numbers And Characters")]
        [TestCase("DepartmentCodeInserted", TestName = "Code Upto 30 characters")]
        [TestCase("C", TestName = "Code as 1 character")]
        public void DepartmentCodeValid(string code)
        {
            //Arrange
            var dept = new DepartmentRequestModel
            {
                //Id = 1,
                Code = code,
                Desc = "Valid Description",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 1,
                //UpdatedById = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(dept).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }


        #endregion

        #region CommodityViewModel

        [Test]
        public void CommodityValidData()
        {
            //Arrange
            var commodity = new CommodityViewModel
            {
                Id = 1,
                Code = "CommodityCode",
                DepartmentId = 1,
                Desc = "CommodityDescription",


            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(commodity).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Code As Blank")]
        [TestCase(" ", TestName = "Code As Blank Spaces")]
        [TestCase("Commodity_CodeCommodity_CodeCommodity_CodeCommodity_Code", TestName = "Name Upto 30 characters")]

        public void CommodityCodeInValid(string code)
        {
            //Arrange
            var commodity = new CommodityViewModel
            {
                Id = 1,
                Code = code,
                DepartmentId = 1,
                Desc = "CommodityDescription"

            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(commodity).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Commodity Desc as Blank")]
        [TestCase(" ", TestName = "Commodity Desc as Blank Spaces")]
        [TestCase("Commodity_DesCommodity_DesCommodity_DesCommodity_DesCommodity_DesCommodity_DesCommodity_Des", TestName = "Desc Upto 80 characters")]

        public void CommodityDescInValid(string desc)
        {
            //Arrange
            var commodity = new CommodityViewModel
            {
                Id = 1,
                Code = "Valid_Code",
                DepartmentId = 1,
                Desc = desc
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(commodity).Count;


            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [TestCase("CommodityTest123{Random Desc}", TestName = "Commodity As Combination Of Numbers And Characters")]
        [TestCase("CommodityDescription_CommodityDescription", TestName = "Commodity Upto 80 characters")]
        [TestCase("C", TestName = "Commodity as 1 character")]
        public void CommodityValidDesc(string desc)
        {
            //Arrange
            var commodity = new CommodityViewModel
            {
                Id = 1,
                Code = "Valid_Code",
                DepartmentId = 1,
                Desc = desc
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(commodity).Count;


            //Assert
            Assert.AreEqual(0, errorcount);

        }

        [TestCase("CodeTest123{Random Desc}", TestName = "Code As Combination Of Numbers And Characters")]
        [TestCase("CommodityCode_CommodityCode", TestName = "Code Upto 30 characters")]
        [TestCase("C", TestName = "Code as 1 character")]
        public void CommodityValidCode(string code)
        {
            //Arrange
            var dept = new DepartmentRequestModel
            {
                //Id = 1,
                Code = code,
                Desc = "CommodityDescription",
                //CreatedAt = DateTime.Now,
                //UpdatedAt = DateTime.Now,
                //CreatedById = 1,
                //UpdatedById = 1
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(dept).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }


        #endregion

        #region Tax
        [Test]
        public void TaxModelValidData()
        {
            //Arrange
            var taxModel = new TaxViewModel
            {
                Id = 0,
                Code = "tax",
                Desc = "tax description",
                Factor = 20.02F,
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(taxModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Tax Code is blank")]
        [TestCase(" ", TestName = "Tax Code is blank spaces")]
        [TestCase("tax tax tax tax tax tax tax tax tax tax tax tax tax tax tax ", TestName = "Tax Code Upto 15 characters")]

        public void TaxCodeInValid(string code)
        {
            //Arrange
            var taxModel = new TaxViewModel
            {
                Id = 0,
                Code = code,
                Desc = "tax description",
                Factor = 20.02F,
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(taxModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Tax Desc is blank")]
        [TestCase(" ", TestName = "Tax Desc is blank spaces")]
        [TestCase("tax description tax description tax description tax description tax description tax description tax description tax description tax description", TestName = "Tax Desc Upto 30 characters")]

        public void TaxDescInValid(string desc)
        {
            //Arrange
            var taxModel = new TaxViewModel
            {
                Id = 0,
                Code = "Code",
                Desc = desc,
                Factor = 20.02F,
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(taxModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [TestCase("TaxTest123{Random Desc}", TestName = "Tax Desc As Combination Of Numbers And Characters")]
        [TestCase("TaxDescription_TaxDescription", TestName = "Tax Desc Upto 30 characters")]
        [TestCase("D", TestName = "Tax Description as 1 character")]
        public void TaxDescValid(string desc)
        {
            //Arrange
            var taxModel = new TaxViewModel
            {
                Id = 0,
                Code = "Code",
                Desc = desc,
                Factor = 20.02F,
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(taxModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [TestCase("T1{Random Code}", TestName = "Tax Code As Combination Of Numbers And Characters")]
        [TestCase("TaxCodeInserted", TestName = "Tax Code Upto 15 characters")]
        [TestCase("C", TestName = "Code as 1 character")]
        public void TaxCodeValid(string code)
        {
            //Arrange
            var taxModel = new TaxViewModel
            {
                Id = 0,
                Code = code,
                Desc = "Valid Desc",
                Factor = 20.02F,
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };


            //Act
            var errorcount = MockViewModelRepository.ValidateModel(taxModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }




        #endregion

        #region APN

        [Test]
        public void ApnModelValidData()
        {
            //Arrange
            var aPNViewModel = new APNRequestModel
            {
                //Id = 0,
                Number = 1234567,
                ProductId = 0000000,
                SoldDate = DateTime.Now,
                Desc = "test Desc"
                //CreatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedAt = DateTime.Now,
                //UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(aPNViewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("", TestName = "APN Desc is blank")]
        [TestCase(" ", TestName = "APN Desc is blank spaces")]
        [TestCase("apn description apn description apn description apn description apn description apn description apn description apn description apn description", TestName = "Tax Desc Upto 80 characters")]

        public void APNDescInValid(string desc)
        {
            //Arrange
            var aPNViewModel = new APNRequestModel
            {
                //Id = 0,
                Number = 1234567,
                ProductId = 0000000,
                SoldDate = DateTime.Now,
                Desc = desc,
                //CreatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedAt = DateTime.Now,
                //UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(aPNViewModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("APNTest123{Random Desc}", TestName = "APN Desc As Combination Of Numbers And Characters")]
        [TestCase("APNDescription_APNDescription", TestName = "APN Desc Upto 80 characters")]
        [TestCase("D", TestName = "APN Description as 1 character")]
        public void APNDescValid(string desc)
        {
            //Arrange
            //Arrange
            var aPNViewModel = new APNRequestModel
            {
                //Id = 0,
                Number = 1234567,
                ProductId = 0000000,
                SoldDate = DateTime.Now,
                Desc = desc,
                //CreatedAt = DateTime.Now,
                //CreatedById = 0,
                //UpdatedAt = DateTime.Now,
                //UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(aPNViewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }


        #endregion

        #region Warehouse

        [Test]
        public void WarehouseModelValidData()
        {
            //Arrange
            var warehouseViewModel = new WarehouseViewModel
            {
                Id = 0,
                Code = "warehouse",
                Desc = "warehouse description",
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(warehouseViewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        public void WarehouseCodeMaxLength()
        {
            //Arrange
            var taxModel = new WarehouseViewModel
            {
                Id = 0,
                Code = "warehouse warehouse warehouse warehouse warehouse warehouse warehouse warehouse warehouse warehouse warehouse ",
                Desc = "warehouse description",
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(taxModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.WarehouseCode, error[0].ErrorMessage);
        }

        [Test]
        public void WarehouseDescMaxLength()
        {
            //Arrange
            var taxModel = new WarehouseViewModel
            {
                Id = 0,
                Code = "warehouse",
                Desc = "warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description warehouse description ",
                CreatedAt = DateTime.Now,
                CreatedById = 0,
                UpdatedAt = DateTime.Now,
                UpdatedById = 0
            };

            //Act
            var error = MockViewModelRepository.ValidateModel(taxModel);

            //Assert
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(ErrorMessages.WarehouseDesc, error[0].ErrorMessage);
        }

        #endregion

        #region  Competition

        #region Trigger
        [Test]
        public void CompTriggerValidData()
        {
            //Arrange
            var viewModel = new CompTriggerRequestViewModel
            {
                TriggerProductGroupID = 101,
                LoyaltyFactor = 1.0F,
                ProductId = 101,
                Desc = "test"
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("", TestName = "Competition Trigger Desc is blank")]
        [TestCase(" ", TestName = "Competition Trigger Desc is blank spaces")]
        [TestCase("description description description description description", TestName = "Competition Trigger Desc Upto 40 characters")]

        public void CompTriggerDescInValid(string desc)
        {
            //Arrange
            var viewModel = new CompTriggerRequestViewModel
            {
                ProductId = 101,
                TriggerProductGroupID = 101,
                LoyaltyFactor = 1.0F,
                Desc = desc
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("CompTest123{Random Desc}", TestName = "Competition Trigger Desc As Combination Of Numbers And Characters")]
        [TestCase("CompDescription_CompDescription", TestName = "Competition Trigger Upto 40 characters")]
        [TestCase("D", TestName = "Competition Trigger Description as 1 character")]
        public void CompTriggerDescValid(string desc)
        {
            //Arrange 
            var viewModel = new CompTriggerRequestViewModel
            {
                ProductId = 101,
                TriggerProductGroupID = 101,
                LoyaltyFactor = 1.0F,
                Desc = desc
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        #endregion

        #region Reward
        [Test]
        public void CompRewardValidData()
        {
            //Arrange
            var viewModel = new CompRewardRequestViewModel
            {
                Count = 1,
                ProductId = 101,
                Desc = "test"
            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        [Test]
        [TestCase("description description description description description", TestName = "Competition Reward Desc Upto 40 characters")]

        public void CompRewardDescInValid(string desc)
        {
            //Arrange
            var viewModel = new CompRewardRequestViewModel
            {
                ProductId = 101,
                Count = 1,
                Desc = desc
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("CompTest123{Random Desc}", TestName = "Competition Reward Desc As Combination Of Numbers And Characters")]
        [TestCase("CompDescription_CompDescription", TestName = "Competition Reward Upto 40 characters")]
        [TestCase("D", TestName = "Competition Reward Description as 1 character")]
        public void CompRewardDescValid(string desc)
        {
            //Arrange 
            var viewModel = new CompRewardRequestViewModel
            {
                ProductId = 101,
                Count = 1,
                Desc = desc
            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }
        #endregion

        [Test]
        public void CompetitionRequestValidData()
        {
            //Arrange
            var viewModel = new CompetitionRequestViewModel
            {
                Code = "test",
                Desc = "test",
                PromotionTypeId = 101,
                SourceId = 101,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                FrequencyId = 101,
                Availibility = "YYYYYYY",
                ZoneId = 101,
                TypeId = 101,
                ResetCycleId = 101,
                LoyaltyFactor = 1.0F,
                ComplDiscount = 2.0F,
                Status = true,
                PosReceiptPrint = false,
                RewardTypeId = 101,
                Discount = 2.0F,
                TriggerTypeId = 101,
                ActivationPoints = 101,
                RewardThreshold = 50,

            };

            //Act
            var errorcount = MockViewModelRepository.ModelValidationCheck(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }


        [Test]
        [TestCase("", TestName = "Competition Code is blank")]
        [TestCase(" ", TestName = "Competition Code is blank spaces")]
        [TestCase("Code Code Code Code", TestName = "Competition Code Upto 15 characters")]

        public void CompetitionCodeInValid(string code)
        {
            //Arrange
            var viewModel = new CompetitionRequestViewModel
            {
                Code = code,
                Desc = "test",
                PromotionTypeId = 101,
                SourceId = 101,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                FrequencyId = 101,
                Availibility = "YYYYYYY",
                ZoneId = 101,
                TypeId = 101,
                ResetCycleId = 101,
                LoyaltyFactor = 1.0F,
                ComplDiscount = 2.0F,
                Status = true,
                PosReceiptPrint = false,
                RewardTypeId = 101,
                Discount = 2.0F,
                TriggerTypeId = 101,
                ActivationPoints = 101,
                RewardThreshold = 50,

            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("CompTest123{R}", TestName = "Competition Code As Combination Of Numbers And Characters")]
        [TestCase("Test_test", TestName = "Competition code Upto 15 characters")]
        [TestCase("D", TestName = "Competition Code as 1 character")]
        public void CompetitionCodeValid(string code)
        {
            //Arrange 
            var viewModel = new CompetitionRequestViewModel
            {
                Code = code,
                Desc = "test",
                PromotionTypeId = 101,
                SourceId = 101,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                FrequencyId = 101,
                Availibility = "YYYYYYY",
                ZoneId = 101,
                TypeId = 101,
                ResetCycleId = 101,
                LoyaltyFactor = 1.0F,
                ComplDiscount = 2.0F,
                Status = true,
                PosReceiptPrint = false,
                RewardTypeId = 101,
                Discount = 2.0F,
                TriggerTypeId = 101,
                ActivationPoints = 101,
                RewardThreshold = 50,

            };


            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }


        [Test]
        [TestCase("", TestName = "Competition Desc is blank")]
        [TestCase(" ", TestName = "Competition Desc is blank spaces")]
        [TestCase("Desc Desc Desc Desc Desc Desc Desc Desc Desc Desc Desc", TestName = "Competition Desc Upto 30 characters")]

        public void CompetitionDescInValid(string desc)
        {
            //Arrange
            var viewModel = new CompetitionRequestViewModel
            {
                Code = "code",
                Desc = desc,
                PromotionTypeId = 101,
                SourceId = 101,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                FrequencyId = 101,
                Availibility = "YYYYYYY",
                ZoneId = 101,
                TypeId = 101,
                ResetCycleId = 101,
                LoyaltyFactor = 1.0F,
                ComplDiscount = 2.0F,
                Status = true,
                PosReceiptPrint = false,
                RewardTypeId = 101,
                Discount = 2.0F,
                TriggerTypeId = 101,
                ActivationPoints = 101,
                RewardThreshold = 50,

            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("CompTest123{R}", TestName = "Competition Desc As Combination Of Numbers And Characters")]
        [TestCase("Test_test", TestName = "Competition Desc Upto 30 characters")]
        [TestCase("D", TestName = "Competition Desc as 1 character")]
        public void CompetitionDescValid(string desc)
        {
            //Arrange 
            var viewModel = new CompetitionRequestViewModel
            {
                Code = "code",
                Desc = desc,
                PromotionTypeId = 101,
                SourceId = 101,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                FrequencyId = 101,
                Availibility = "YYYYYYY",
                ZoneId = 101,
                TypeId = 101,
                ResetCycleId = 101,
                LoyaltyFactor = 1.0F,
                ComplDiscount = 2.0F,
                Status = true,
                PosReceiptPrint = false,
                RewardTypeId = 101,
                Discount = 2.0F,
                TriggerTypeId = 101,
                ActivationPoints = 101,
                RewardThreshold = 50,

            };


            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }


        [Test]
        [TestCase("Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility Availibility ", TestName = "Competition Availibility Upto 500 characters")]

        public void CompetitionAvailibilityInValid(string avail)
        {
            //Arrange
            var viewModel = new CompetitionRequestViewModel
            {
                Code = "code",
                Desc = "Desc",
                PromotionTypeId = 101,
                SourceId = 101,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                FrequencyId = 101,
                Availibility = avail,
                ZoneId = 101,
                TypeId = 101,
                ResetCycleId = 101,
                LoyaltyFactor = 1.0F,
                ComplDiscount = 2.0F,
                Status = true,
                PosReceiptPrint = false,
                RewardTypeId = 101,
                Discount = 2.0F,
                TriggerTypeId = 101,
                ActivationPoints = 101,
                RewardThreshold = 50,

            };

            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(1, errorcount);
        }

        [Test]
        [TestCase("CompTest123{R}", TestName = "Competition Availibility As Combination Of Numbers And Characters")]
        [TestCase("Test_test", TestName = "Competition Availibility Upto 500 characters")]
        [TestCase("D", TestName = "Competition Availibility as 1 character")]
        public void CompetitionAvailibilityValid(string avail)
        {
            //Arrange 
            var viewModel = new CompetitionRequestViewModel
            {
                Code = "code",
                Desc = "desc",
                PromotionTypeId = 101,
                SourceId = 101,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                FrequencyId = 101,
                Availibility = avail,
                ZoneId = 101,
                TypeId = 101,
                ResetCycleId = 101,
                LoyaltyFactor = 1.0F,
                ComplDiscount = 2.0F,
                Status = true,
                PosReceiptPrint = false,
                RewardTypeId = 101,
                Discount = 2.0F,
                TriggerTypeId = 101,
                ActivationPoints = 101,
                RewardThreshold = 50,

            };


            //Act
            var errorcount = MockViewModelRepository.ValidateModel(viewModel).Count;

            //Assert
            Assert.AreEqual(0, errorcount);
        }

        #endregion


    }
}