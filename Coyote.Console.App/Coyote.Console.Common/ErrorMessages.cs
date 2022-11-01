namespace Coyote.Console.Common
{
    public static class ErrorMessages
    {
        #region APN

        public const string NoAPNFound = "No APN Found";
        public const string APNNotFound = "APN not Found";
        public const string APNNumbDuplicate = "This APN number already exists";
        public const string APNNumberDuplicate = "Can not add same APN number twice.";
        public const string APNProdNotExist = "This Product doesn't exist.";
        public const string APNDeleted = "APN Deleted.";
        public const string APNCode = "APN Code cannot be longer than 80 characters";

        #endregion

        #region Commodity
        public const string CommodityCode = "Commodity code cannot be longer than 30 characters";
        public const string CommodityName = "Commodity Name cannot be longer than 80 characters";

        public const string CommodityCodeDuplicate = "Commodity Code already exists";
        public const string CommodityIdNotFound = "Commodity Id does not exist in the system, please try with some other id";
        public const string CommodityCreatedSuccess = "Commodity Created successfully";
        public const string CommodityUpdatedSuccess = "Commodity Updated successfully";
        public const string CommodityDeletedSuccess = "Commodity Deleted successfully";
        public const string CommodityNotFound = "Commodity Not Found";
        public const string CommodityFound = "Commodity Records Found";
        #endregion

        #region Created/UpdatedBy

        public const string CreatedByRequired = "Created By Id can not be 0";
        public const string UpdatedByRequired = "Updated By Id can not be 0";
        public const string CreatedUpdatedRequired = "Created By Id and Updated By Id can not be 0";
        public const string UserUpdatedRequired = "Updated By Id can not be 0";
        #endregion

        #region Department
        public const string DepartmentCode = "Department code cannot be longer than 30 characters";
        public const string DepartmentName = "Department Name cannot be longer than 80 characters";

        public const string DepartmentCodeDuplicate = "Department Code already exists";
        public const string DepartmentIdNotFound = "Department Id does not exist in the system, please try with some other id";
        public const string DepartmentCreatedSuccess = "Department Created successfully";
        public const string DepartmentUpdatedSuccess = "Department Updated successfully";
        public const string DepartmentDeletedSuccess = "Department Deleted successfully";
        public const string DepartmentNotFound = "Department Not Found";
        public const string DepartmentFound = "Department Records Found";

        #endregion

        #region EmailTemplates
        public const string EmailTemplateNameLength = "Name cannot be longer than 100 characters";
        public const string EmailTemplateDisplayNameLength = "Display Name cannot be longer than 100 characters";
        public const string EmailTemplateSubjectLength = "Subject cannot be longer than 100 characters";
        public const string EmailTemplateBodyLength = "Body cannot be longer than 1000 characters";
        #endregion

        #region MasterList
        public const string ListCode = "List Code cannot be longer than 50 characters";
        public const string ListName = "List Name cannot be longer than 100 characters";

        public const string ListCodeRequired = "List Code is required.";
        public const string ListNameRequired = "List Name is required.";
        public const string MasterListCodeDuplicate = "List Code already exists";
        public const string MasterListNameDuplicate = "List Name already exists";
        public const string MasterListIdNotFound = "Master List Id does not exist in the system, please try with some other id";

        public const string MasterListCreatedSuccess = "Master List Created successfully";
        public const string MasterListUpdatedSuccess = "Master List Updated successfully";
        public const string MasterListDeletedSuccess = "Master List Deleted successfully";
        public const string MasterListNotFound = "Master List Not Found";
        public const string MasterListInvalid = "Invalid Master List Id";

        public const string MasterListFound = "Master List Records Found";
        public const string MasterListAccess = "Item does not have access to write.";
        #endregion

        #region MasterListItem
        public const string ListItemCode = "Item Code cannot be longer than 30 characters";
        public const string ListItemName = "Item Name cannot be longer than 80 characters";

        public const string ListItemCodeRequired = "Item Code is required.";
        public const string ListItemNameRequired = "Item Name is required.";
        //public const string ListItemCodeDuplicate = "Selected List and Code combination already exists in the system.";
        //public const string ListItemNameDuplicate = "Selected List and Name combination already exists in the system";

        public const string ListItemCodeDuplicate = "Code already exists.";
        public const string ListItemNameDuplicate = "Description already exists.";

        public const string ListItemIdNotFound = "List Item Id does not exist in the system, please try with some other id";

        public const string ListItemCreatedSuccess = " List Item Created successfully";
        public const string ListItemUpdatedSuccess = "List Item Updated successfully";
        public const string ListItemDeletedSuccess = "List Item Deleted successfully";
        public const string ListItemNotFound = "List Item Not Found";

        public const string ListItemFound = "List Item Records Found";
        #endregion

        #region ModuleActions
        public const string ModuleActionsNameRequired = "Module Action Name is Required";
        public const string ModuleActionsNameLength = "Module Action Name cannot be longer than 100 characters";
        public const string ModuleActionsActionRequired = "Module Actions Action is Required";
        public const string ModuleActionNotFound = "Module Action not found";
        #endregion

        #region Product
        public const string ProductDescLength = "Product Description cannot be longer than 50 characters";
        public const string ProductPosDescLength = "Product Pos Description cannot be longer than 20 characters";
        public const string ProductFreightLength = "Product Freight cannot be longer than 15 characters";
        public const string ProductSizeLength = "Product Size cannot be longer than 8 characters";
        public const string ProductInfoLength = "Additional Info cannot be longer than 50 characters";
        public const string ProductHostNumberLength = "Product Host Number cannot be longer than 15 characters";
        public const string ProductReplicateLength = "Product Replicate cannot be longer than 15 characters";
        public const string ProductHostItemTypeLength = "Product Host Item Type cannot be longer than 1 character";

        public const string ProductNotFound = "Product Not Found";
        public const string ProductNumerDuplicate = "Product Number already exists";
        public const string ProductIdReq = "Product Id is required";
        public const string ProductDeletedSuccess = "Product Deleted.";
        public const string ProductDepartment = "This department does not exist.";
        public const string ProductParent = "Invalid Parent Product.";
        public const string ProductParentSame = "Product can not be its own Parent.";

        public const string OutletProductStore = "Invalid store in Outlet Product";
        public const string OutletProductSupplier = "Invalid Supplier in Outlet Product_4535";
        public const string ProductSupplier = "Invalid Supplier in Supplier Product";
        public const string ProductSupplierDuplicate = "Can not add same Supplier for Supplier Product twice.";
        #endregion

        #region ResetPassword
        public const string NewUserPassword = "Password should contain min 8 characters with combination of caps alpha numeric & 1 special character.";
        public const string PasswordUpdateFailed = "Your passwords don’t match. Please enter your password again to confirm it.";
        public const string PasswordUpdated = "Password Updated.";
        public const string ConfirmPassNotMatch = "New password and confirm password do not match.";
        public const string UserNotExistWIthEmail = "User not exists with given email id - ";
        public const string UnableToSendMail = "Unable To Send Mail";
        public const string LoginFailed = "Invalid email/password combination";
        public const string LoginSuccess = "Login Success! Redirecting…";
        public const string PasswordUpdateSuccess = "Your Password has been Successfully Changed.";
        public const string MailSendSeccessfully = "We have emailed you password reset link.";
        public const string PasswordExpired = "Your passwords has been Expired.";
        public const string InValidEmailAddress = "Please enter valid email address";
        public const string InValidRefreshTokens = "Invalid token.";
        public const string InValidPassword = "Please enter valid password";
        #endregion

        #region Roles
        public const string RoleCode = "Code cannot be longer than 30 characters";
        public const string RoleName = "Name cannot be longer than 50 characters";
        public const string RoleType = "Type cannot be longer than 50 characters";

        public const string RoleNameDuplicate = "Role Name already exists";
        public const string RoleCodeDuplicate = "Role Code already exists";

        public const string RoleNameRequired = "Role Name is required";
        public const string RolePermissionRequired = "Role Permission Set is required";
        public const string RoleTypeRequired = "Role Type is required";
        public const string RoleCodeRequired = "Role Code is required";

        public const string RoleIdNotFound = "Role Id does not exist in the system, please try with some other id";

        public const string RoleCreatedSuccess = "Role Created successfully";
        public const string RoleUpdatedSuccess = "Role Updated successfully";
        public const string RoleDeletedSuccess = "Role Deleted successfully";
        public const string RoleNotFound = "Role Not Found";

        public const string RoleFound = "Role Records Found";

        #endregion

        #region Store

        public const string store = "store  cannot be longer than 30 characters";
        public const string Zone = "Zone  cannot be longer than 30 characters";
        public const string StoreGroup = "Store_Group  cannot be longer than 30 characters";
        public const string StoreAddress1 = "Store_Address1  cannot be longer than 30 characters";
        public const string StoreAddress2 = "Store_Address2  cannot be longer than 30 characters";
        public const string StoreAddress3 = "Store_Address3  cannot be longer than 30 characters";
        public const string StorePhoneNumber = "Store_Phone_Number  cannot be longer than 15 characters";
        public const string StoreFax = "Store_Fax  cannot be longer than 15 characters";
        public const string StorePostCode = "Store_Post_Code  cannot be longer than 4 characters";
        public const string StoreStatus = "Store_Status  cannot be longer than 10 characters";
        public const string StoreDesc = "Store_Desc  cannot be longer than 40 characters";
        public const string StoreCode = "Store Code  cannot be longer than 30 characters";
        public const string StoreCodeReq = "Store Code is Required.";
        public const string StoreEmail = "Store_Email  cannot be longer than 50 characters";
        public const string StorePriceZone = "Store_Price_Zone  cannot be longer than 15 characters";
        public const string StoreSellingInd = "Store_Selling_Ind  cannot be longer than 3 characters";
        public const string StoreStockInd = "Store_Stock_Ind  cannot be longer than 3 characters";
        public const string StoreDelName = "Store_Del_Name  cannot be longer than 30 characters";
        public const string StoreDelAddr1 = "Store Del Addr_1  cannot be longer than 30 characters";
        public const string StoreDelAddr2 = "Store Del Addr_2  cannot be longer than 30 characters";
        public const string StoreDelAddr3 = "Store Del Addr_3  cannot be longer than 30 characters";
        public const string StoreDelPostCode = "Store_Del_Post_Code  cannot be longer than 4 characters";
        public const string StoreCostType = "Store_Cost_Type  cannot be longer than 10 characters";
        public const string StoreAbn = "Store_Abn  cannot be longer than 15 characters";

        public const string StoreEntityNumber = "Store Entity Number  cannot be longer than 3 characters";
        public const string StorePriceLevelDesc = "Store Price Level Description  cannot be longer than 30 characters";
        public const string StoreCostZone = "Store Cost Zone  cannot be longer than 15 characters";
        public const string StoreLatitude = "Store Latitude  cannot be longer than 64 characters";
        public const string StoreLongitude = "Store Longitude  cannot be longer than 64 characters";
        public const string StoreOpenHours = "Store Open Hours  cannot be longer than 256 characters";
        public const string StoreNameOnApp = "Stor Name On App  cannot be longer than 64 characters";
        public const string StoreAddressOnApp = "Store Address On App  cannot be longer than 64 characters";
        public const string StoreLabelTypeNormal = "Store Label Type Normal  cannot be longer than 15 characters";
        public const string StoreLabelTypePromo = "Store Label_Type Promo  cannot be longer than 15 characters";
        public const string StoreLabelTypeShort = "Store Label_Type Short  cannot be longer than 15 characters";

        public const string StoreId = "Store Id is required.";
        public const string OutletId = "Outlet Id is required.";
        public const string StoreCodeDuplicate = "This Store Code already exists";
        public const string StoreNotFound = "Store does not exist";
        public const string OutletNotFound = "Outlet does not exist";
        public const string StoreFound = "Store Found";
        public const string NoStoreFound = "No Stores Found";
        public const string StoreIdDuplicate = "Store Id already exists";
        public const string StoreUpdated = "Store Updated";
        public const string StoreDeletedSuccess = "Store Deleted";
        public const string StoreAdded = "Store Added.";
        public const string OutletRequired = "Outlet Required.";
        public const string StoreInactive = "Can not select Inactive Store";
        #endregion

        #region StoreGroup
        public const string StoreGroupCode = "Group Code cannot be longer than 30 characters";
        public const string StoreGroupName = "Store Group Description cannot be longer than 100 characters";
        public const string StoreGroupNameReq = "Store Group Description is required";
        public const string StoreGroupDesc = "Group Descriprion is required";
        public const string StoreDefaultAccess = "Default Access cannot be longer than 30 characters";
        public const string StoreGroupAccess = "Group Access cannot be longer than 30 characters";

        public const string StoreGroupDuplicate = "This Store Group Code already exists";
        public const string StoreGroupId = "Store Group Id is required.";

        public const string StoreGroupNotFound = "This Store Group does not exist";
        public const string NoStoreGroupFound = "No Store Group Found.";
        public const string StoreGroupFound = "Store Group found";
        public const string StoreGroupDeleted = "Store Group Deleted";
        public const string InvalidLatitude = "Latitude must be between -90 and 90 degrees.";
        public const string InvalidLogitude = "Longitude must be between -180 and 180 degrees.";
        public const string InvalidShelfLabel = "Invalid Shelf Label Type.";
        public const string InvalidShortLabel = "Invalid Short Label Type.";
        public const string InvalidPromoLabel = "Invalid Promotion Label Type.";
        public const string InvalidWarehouse = "Invalid Warehouse Id.";
        public const string InvalidPriceZoneId = "Invalid Price Zone Id.";
        public const string InvalidCostZoneId = "Invalid Cost Zone Id.";
        public const string InvalidRoyaltyCount = "Can not add more than five Sale range for one type. .";
        public const string InvalidRoyaltyScale = "Royalty Sales from can not be higher than Sales To.";
        public const string InvalidAdvRoyaltyScale = "Advertising Sales from can not be higher than Sales To.";
        public const string InvalidRoyaltyScalePrev = "Royalty From Sales must be lower than previous To Sales.";
        public const string InvalidRoyaltyAdvScalePrev = "Advertising From Sales must be lower than previous To Sales.";
        public const string InvalidRoyaltyPerct = "Percentage is required.";
        public const string InvalidRoyaltyType = "Invalid Royalty Scale Type";
        public const string InvalidRoyaltyScaaleRepeat = "Can not add same scale twice.";
        #endregion

        #region Supplier

        public const string SuppCode = "Code cannot be longer than 30 characters";
        public const string SuppDesc = "Desc cannot be longer than 80 characters";
        public const string SuppAddress1 = "Address_1 cannot be longer than 80 characters";
        public const string SuppAddress2 = "Address_2 cannot be longer than 80 characters";
        public const string SuppAddress3 = "Address_3 cannot be longer than 80 characters";
        public const string SuppPhone = "Phone cannot be longer than 20 characters";
        public const string SuppFax = "Fax cannot be longer than 20 characters";
        public const string SuppEmail = "Email cannot be longer than 30 characters";
        public const string SuppABN = "ABN cannot be longer than 15 characters";
        public const string SuppUpdateCost = "Update_Cost cannot be longer than 3 characters";
        public const string SuppPromoCode = "Promo_Code cannot be longer than 30 characters";
        public const string SuppContactName = "Contact_Name cannot be longer than 30 characters";
        public const string SuppCostZone = "Cost_Zone cannot be longer than 30 characters";
        public const string SuppGSTFreeItemCode = "GST_Free_Item_Code cannot be longer than 30 characters";
        public const string SuppGSTFreeItemDesc = "GST_Free_Item_Desc cannot be longer than 80 characters";
        public const string SuppGSTInclItemCode = "GST_Incl_Item_Code cannot be longer than 30 characters";
        public const string SuppGSTInclItemDesc = "GST_Incl_Item_Desc cannot be longer than 80 characters";
        public const string SuppXeroName = "Xero_Name cannot be longer than 50 characters";


        public const string NoSupplierFound = "No Supplier found.";
        public const string SupplierFound = "Supplier Found";
        public const string SupplierNotFound = "Supplier Not Found";
        public const string SupplierDuplicateId = "This Supplier Id already exists.";
        public const string SupplierDuplicateCode = "This Supplier Code already exists.";
        public const string SupplierId = "Supplier Id is required.";
        public const string SupplierUpdated = "Supplier Updated.";
        public const string SupplierAdded = "Supplier Added.";
        public const string SupplierDeleted = "Supplier Deleted.";
        public const string SupplierNotDeleted = "Supplier is in use, can not be deleted.";
        public const string SupplierProductSupplier = "Supplier is in use in Supplier Product, can not be deleted.";
        public const string ProductsSupplier = "Supplier is in use in Products, can not be deleted.";
        public const string OutletProductSupplierDelete = "Supplier is in use in Outlet Product, can not be deleted.";
        public const string PromotionBuyingSupplierDelete = "Supplier is in use in Promotions, can not be deleted.";
        public const string OrderHeaderSupplier = "Supplier is in use Orders, can not be deleted.";
        public const string OrderDetailCheaperSupplier = "Supplier is in use in Order-Details, can not be deleted.";
        public const string OutletSupplierSettings = "Supplier is in use in Outlet-Supplier settings, can not be deleted.";
        public const string GLAccountSupplier = "Supplier is in use in GLAccounts, can not be deleted.";
        public const string SupplierOrderingScheduleSupplier = "Supplier is in use in Supplier Ordering Schedules, can not be deleted.";

        public const string SupplierRequired = "Supplier Required.";
        #endregion

        #region SupplierProduct
        public const string SupplierProductDescLength = "Supplier Product Description cannot be longer than 50 characters";
        public const string NoSupplierProductFound = "No Supplier Product found.";
        public const string SupplierProductNotFound = "Supplier Product Not Found";
        public const string SupplierProductFound = "Supplier Product Found";
        public const string SupplierProductDeleted = "Supplier Product Deleted.";
        public const string SupplierProductId = "Supplier Product Id is required.";
        public const string SupplierProductDuplicate = "This Supplier Product already exists.";
        public const string SupplierProductItemCodeLength = "Supplier Product Item Code can't be longer than 15 characters";
        #endregion

        #region OutletProduct
        public const string NoOutletProductFound = "No Outlet Product found.";
        public const string OutletProductDuplicate = "This Outlet Product already exists.";
        public const string OutletProductDeleted = "Outlet Product Deleted.";
        public const string OutletProductNotFound = "Outlet Product Not Found";
        public const string OutletProductId = "Outlet Product Id is required.";
        public const string StoreIdRequired = "Store Id is required.";
        public const string SetMinOnHandNotFound = "Sales data not available for the entered days history.";
        #endregion

        #region Product
        public const string NoProductFound = "No Product found.";
        public const string ProductDuplicate = "Product already exists";
        public const string Number = " Product number is required";
        public const string DeptId = "Dept Id is required.";
        public const string ProductDeleted = "Product Deleted";
        public const string OutletProdformat = "Outlet Product or Supplier Product is not in correct format.";
        public const string MaximumDays = "Allows maximum to 360 days.";
        #endregion

        #region Tax
        public const string TaxCode = "Tax code cannot be longer than 15 characters";
        public const string TaxDesc = "Tax Description cannot be longer than 30 characters";

        public const string TaxIdNotFound = "This TAX Id does not exist in the system, please try with some other id";
        public const string TaxNotFound = "Taxes not found";
        public const string NoTaxFound = "No Taxes Found";
        public const string TaxAdded = "Tax Added";
        public const string TaxUpdated = "Tax Updated";
        public const string TaxDeleted = "Tax Deleted";
        public const string TaxCodeDuplicate = "This Tax Code already exists.";


        #endregion

        #region User_Roles
        public const string UserRoleCode = "User Role Name cannot be longer than 30 characters";
        public const string UserRoleName = "User Role Name cannot be longer than 50 characters";
        public const string UserRoleType = "User Role Type cannot be longer than 50 characters";

        public const string UserRoleCodeRequired = "Please enter User Role Code";
        public const string UserRoleNameRequired = "Please enter User Role Name";
        public const string UserRoleTypeRequired = "Please enter User Role Type";

        public const string UserRoleNameDuplicate = "User Role Name already exists";
        public const string UserRoleCodeDuplicate = "User Role Code already exists";

        public const string UserRoleIdRequired = "User Role Id is required";
        public const string UserRoleNotFound = "This User Role does not exist";


        public const string UserRoleIdNotFound = "User Role Id does not exist in the system, please try with some other id";
        public const string UserRoleCreatedSuccess = "User Role created successfully";
        public const string UserRoleCreatedFailure = "User Role not created.";
        public const string UserRoleUpdateFailure = "User Role not updated.";
        public const string UserRoleUpdatedSuccess = "User Role updated successfully";
        public const string UserRoleDeletedSuccess = "User Role deleted successfully";
        //public const string UserRoleNotFound = " User Role Not Found";
        public const string UserRoleDefaultRoleRequired = "Default User Role  is required";

        public const string UserRoleFound = "User Role Records Found";

        public const string InternalError = "Internal server error occured";
        public const string InternalErrorUserAdded = "User added successfully, but roles having some problem.";

        public const string TokenExpired = "Token Expired on : ";

        #endregion

        #region User Error Messages

        public const string UserFirstName = "First Name cannot be longer than 20 characters";
        public const string UserMiddleName = "Middle Name cannot be longer than 20 characters";
        public const string UserLastName = "Last Name cannot be longer than 20 characters";
        public const string UserPassword = "Password cannot be longer than 20 characters";
        public const string UserEmail = "Email address cannot be longer than 50 characters";
        public const string UserGender = "Gender cannot be longer than 13 characters";
        public const string UserStatus = "Status cannot be longer than 8 characters";
        public const string UserAddress = "Address cannot be longer than 30 characters";
        public const string UserMobileNo = "Mobile No cannot be longer than 15 characters";
        public const string UserPhoneNo = "Phone No cannot be longer than 15 characters";
        public const string UserPostal = "Postal Code cannot be longer than 4 characters";
        public const string UserEmailForLogin = "Username/Email is required";
        public const string UserPasswordForLogin = "Password is required";

        public const string UserEmailDuplidate = "Email address already exists. Please try with new email address!.";

        public const string UserFirstNameRequired = "Please enter First Name!";
        public const string UserEmailAddressRegularExpression = "Not a valid email, Please enter valid email address!";
        public const string UserEmailAddressRequired = "Please enter valid email address!";
        public const string UserPasswordRequired = "Please enter valid Password!";
        public const string UserNotFound = "This User Id does not exist";
        public const string UserDetailNotFound = "This User does not exist";
        public const string UserIdNotFound = "User Id does not exist in the system, please try with some other id";
        public const string UserCreatedSuccess = "User Created successfully";
        public const string UserUpdatedSuccess = "User Updated successfully";
        public const string UserDeletedSuccess = "User Deleted successfully";
        //public const string UserNotFound = " User Not Found";
        public const string UsernameNameLength = "User Name cannot be larger than 100 characters";
        public const string UserNameRequired = "User Name is Required";
        public const string UserNameDuplicate = "User Name already exists. Please try with new User Name";

        public const string UserZoneInvaid = "This Zone Id is not valid.";
        public const string UserDOBRequired = "Date of Birth is required.";
        public const string UserDOBInvalid = "Date of Birth is invalid.";
        public const string UserStoreInvaid = "This Store Id is not valid.";
        public const string UserFound = "User Records Found";
        public const string DefaultRoleInvalid = "Select default role from roles list.";
        #endregion

        #region Warehouse
        public const string WarehouseCode = "Warehouse code cannot be longer than 15 characters";
        public const string WarehouseDesc = "Warehouse Description cannot be longer than 50 characters";


        public const string WarehouseIdNotFound = "This Warehouse Id does not exist in the system, please try with some other id";
        public const string WarehouseNotFound = "Warehouse not found";
        public const string NoWarehouseFound = "No Warehouse Found";
        public const string WarehouseAdded = "Warehouse Added";
        public const string WarehouseUpdated = "Warehouse Updated";
        public const string WarehouseDeleted = "Warehouse Deleted";
        public const string WarehouseCodeDuplicate = "This Warehouse Code already exists.";
        public const string WarehouseSupplierNotFound = "This supplier id does not exist.";
        public const string WarehouseHostNotFound = "This Host Format Id does not exist.";



        #endregion

        #region Zone
        public const string ZoneCode = "Zone cannot be longer than 30 characters";
        public const string ZoneDesc = "Type cannot be longer than 80 characters";
        public const string ZoneDashboard = "Type cannot be longer than 30 characters";
        #endregion

        #region ZoneOutLet
        public const string ZOutCode = "Zone Outlet code cannot be longer than 30 characters";
        public const string ZOutName = "Zone Outlet Name cannot be longer than 80 characters";
        public const string ZOutDesc = "Zone Outlet Description cannot be longer than 80 characters";



        public const string NoZoneOutletFound = "No Zone Outlet Found.";
        public const string ZoneOutletFound = "Zone Outlet Found.";
        public const string ZoneOutletNotFound = "Zone Outlet not found.";
        public const string ZoneNotFound = "Zone not found.";
        public const string ZoneInactive = "Can not use Inactive Zone.";
        public const string ZoneOutletDuplicateId = "This Zone Outlet already exists.";
        public const string ZoneOutletDuplicateCode = "This Zone Code already exists.";
        public const string ZoneOutletAdded = "Zone Outlet Added.";
        public const string ZoneOutletNotAdded = "Zone Outlet can not be added.";
        public const string ZoneOutletId = "Zone Outlet Id is required.";
        public const string ZoneOutletUpdated = "Zone Outlet Updated.";
        public const string ZoneOutletDeleted = "Zone Outlet Deleted.";
        public const string InvalidStore = "Please enter a valid store Id.";

        #endregion

        #region ZoneType
        public const string ZoneTypeLabel = "Type cannot be longer than 100 characters";
        #endregion

        #region Promotion
        public const string PromotionCodeLength = "Promotion Code cannot be longer than 15 characters";
        public const string PromotionCodeLengthDB = "Promotion Code cannot be longer than 20 characters";
        public const string PromotionDescLength = "Promotion Description cannot be longer than 40 characters";
        public const string PromotionRptGroupLength = "Promotion Rpt Group cannot be longer than 15 characters";
        public const string PromotionAvailibilityLength = "Promotion Availability cannot be longer than 500 characters";
        public const string PositiveNumberOnly = "Only Positive numbers are allowed.";
        //public const string PromotionImagePathLength = "Promotion ImagePath cannot be longer than 1000 characters";



        public const string PromotionNotFound = "No Promotion Found";
        public const string PromotionCompetitionCopy = "Can not clone Competition.";
        public const string PromotionCodeDuplicate = "Promotion Code already exists.";
        public const string PromotionTypeNotFound = "Promotion Type Not Found";
        public const string PromotionSourceNotFound = "Promotion Source Not Found";
        public const string PromotionZoneNotFound = "Promotion Zone Not Found";
        public const string PromotionFrequencyNotFound = "Promotion Frequency Not Found";
        public const string PromotionDeleted = "Promotion Deleted";
        public const string PromotionDetailDeleted = "Promotion Detail Deleted";
        public const string PromotionIdMismatch = "Promotion Selected and promotion Added are Different";
        public const string PromotionBuyingProductDuplicate = "Can't add same product twice in a promotion";
        public const string promotionDetailNotFound = "Promotion Detail Not Found";
        public const string PromotionDetailUpdated = "Promotion Details updated";
        public const string PromotionOfferDetailsRequired = "Promotion Offer Details Required.";
        public const string PromotionNotSaved = "Promotion could not be saved.";
        public const string MixMatchRequired = "Promotion Mixmatch Details are required.";
        public const string OfferRequired = "Promotion Offer Details are required.";
        public const string PromotionRequired = "Promotion Details are required.";
        public const string InvalidSource = "Invalid Promotion Source";
        #endregion


        #region PromotionProduct
        public const string PromotionProductDescLength = "Promotion for product Desc cannot be longer than 40 characters";
        public const string PromotionProductActionLength = "Promotion for product Action cannot be longer than 4 characters";
        public const string PromotionProductHostPromoTypeLength = "Promotion for product Host Promo Type cannot be longer than 5 characters";

        #endregion


        #region Cashier
        public const string CashierFirstNameLength = "Cashier FirstName cannot be longer than 20 characters";
        public const string CashierSurnameLength = "Cashier Surname cannot be longer than 30 characters";
        public const string CashierAddressLength = "Cashier Address cannot be longer than 30 characters";
        public const string CashierPostcodeLength = "Cashier Postcode cannot be longer than 4 characters";
        public const string CashierPhoneLength = "Cashier Phone cannot be longer than 15 characters";
        public const string CashierEmailLength = "Cashier Email cannot be longer than 50 characters";
        public const string CashierGenderLength = "Cashier Gender cannot be longer than 6 characters";
        public const string CashierPasswordLength = "Cashier Password cannot be longer than 10 characters";
        public const string CashierWristBandIndLength = "Cashier WristBandInd cannot be longer than 3 characters";
        public const string CashierDispnameLength = "Cashier Display name cannot be longer than 20 characters";
        public const string CashierDispname = "Display name is Required.";
        public const string CashierLeftHandTillIndLength = "Cashier Cashier LeftHand Till Ind cannot be longer than 3 characters";
        public const string CashierFuelUserLength = "Cashier Fuel User cannot be longer than 10 characters";
        public const string CashierFuelPassLength = "Cashier Fuel Pass cannot be longer than 10 characters";
        public const string CashierPassword = "Password is Required.";
        public const string CashierAlreadyExist = "Cashier already exists";
        public const string InvalidCashier = "Invalid store id";
        //  public const string InvalidStore = "Invalid store id";
        public const string InvalidZone = "Invalid zone id";
        public const string InvalidTypeId = "Invalid Type id";
        public const string InvalidAccessId = "Invalid Access level id";
        public const string CashierIdRequired = "Id is required";
        public const string CashierNotFound = "Cashier Not Found";
        public const string CashierDeleted = "Cashier Deleted";


        #endregion

        #region Keypad
        public const string KeypadCodeLength = "Keypad Code length cannot be longer than 30 characters";
        public const string KeypadDescLength = "Keypad Desc length cannot be longer than 50 characters";

        public const string KeypadLevelIndCount = "Invalid Level Index Count";
        public const string KeypadButtonIndCount = "Invalid Button Index Count";
        public const string InvalidOutletId = "Invalid outlet Id";
        public const string OutletIdReq = "Outlet Id Required.";
        public const string KeyPadDuplicate = "Keypad already exists";
        public const string KeypadNotFound = "Keypad not found";
        public const string KeypadDeleted = "Keypads Deleted";
        public const string InvalidKeypadId = "Invalid Ids";
        public const string InvalidKeypad = "Invalid keypad Id";
        public const string InvalidCopyKeypad = "Invalid copy keypad Id";

        #endregion

        #region Till
        public const string TillCodeLength = "Till Code length cannot be longer than 15 characters";
        public const string TillDescLength = "Till Desc length cannot be longer than 50 characters";
        public const string TillSerailNoLength = "Till SerailNo length cannot be longer than 15 characters";

        public const string InvalidTillId = "Till Id is invalid";
        public const string TillNotFound = "Till not found";
        public const string TillDeleted = "Till Deleted";
        public const string TillDuplicate = "Till already exists";
        public const string TillInvalidReq = "Invalid till data";
        public const string TillInvalidSyncSelect = "Select one from[Product / Cashier / Keypad / Remove Sync Request] to proceed";
        public const string TillInvalidRemoveSelect = "Can not select others if Remove Sync Request is Selected.";
        public const string TillInvalidStoreSelect = "Select Stores to Sync Tills.";
        public const string TillInvalidStore = "Selected Store Id not Found.";





        #endregion

        #region OrderHeader
        public const string OrderRefLength = "Order Reference length cannot be longer than 15 characters";
        public const string OrderDeliveryNoLength = "Order Delivery No length cannot be longer than 30 characters";
        public const string OrderInvoiceNoLength = "Order Invoice No length cannot be longer than 15 characters";

        public const string OrderNotFound = "Order not found";
        public const string OrderNotDelivery = "Delivery details can only be saved if Order Type is Delivery.";
        public const string OrderNotInvoice = "Invoice details can only be saved if Order Type is Invoice.";
        public const string OrderHeaderDuplicate = "Order Already Exists";
        public const string CreationTypeNotFound = "Order creation type is not found";
        public const string TypeNotFound = "Type Invalid";
        public const string OrderTypeNotFound = "Order Type Invalid";
        public const string StatusNotFound = "Status Invalid";
        public const string OrderIncomplete = "Incomplete Order";
        public const string OrderDeleted = "Order Deleted";
        public const string OrderDetailsDeleted = "Order Details Deleted";
        public const string EDILIONEmailNotSent = "EDI for Lion Supplier Email not sent";
        public const string EDICocaColaSFTPNotSent = "EDI for Coca Cola Supplier File not sent";
        public const string EDIDistributorSFTPNotSent = "EDI for Distributor Supplier File not sent";
        public const string TransferOrderStoreAsSupplier = "For Transfer Order Outlet as Supplier is required";
        public const string OrderSupplierRequired = "Supplier is Required";
        public const string NotEnoughStockForTransfer = "Stock On hand for Product: {0} in Store: {1} is: {2}";
        public const string OrderNotSaved = "Error saving order header.";

        public const string OrderFilePathNotFound = "Order file Path not found for this outlet";
        #endregion

        #region OrderDetail
        public const string OrderBuyPromoCodeLength = "Order BuyPromoCode can not be longer than 15 characters";
        public const string OrderSalePromoCodeLength = "Order SalePromoCode length can not be longer than 30 characters";
        public const string OrdereSalePromoEndDateLength = "Order SalePromoEndDate length can not be longer than 32 characters";

        #endregion

        #region PrintLabelType
        public const string PrintLabelTypeCodeLength = "Print Label Type Code can not be longer than 15 characters";
        public const string PrintLabelTypeCodeReq = "Print Label Type Code is required.";
        public const string PrintLabelTypeDescLength = "Print Label Type Description can not be longer than 15 characters";
        public const string PrintLabelTypeDescReq = "Print Label Type Description is required.";

        public const string LabelNotFound = "Label Type Not Found";
        public const string LabelDeleted = "Label Deleted";
        public const string LabelIdReq = "Label Id is required";
        public const string LabelPerPage = "Label per page is required";
        public const string LabelDuplicate = "Label already exists";
        public const string LabelCodeTypeInvalid = "Label Code Type Id is Invalid";
        public const string InvalidProductRange = "Product Number must be greater than 0.";
        public const string InvalidPDEImport = "PDE File is in wrong format.";
        #endregion

        #region KeypadLevel
        public const string KeypadLevelDescLength = "KeypadLevel Desc length can not be longer than 50 characters";
        public const string KeypadLevelInvalidIndex = "Keypad Level Count Can not be less than 1 or more than 100.";
        #endregion

        #region KeypadButton

        public const string KeypadButtonInvalidIndex = "Keypad Button count can not be less than 1 or more than 3600.";

        public const string KeypadButtonTypeLength = "Keypad Button Type length can not be longer than 30 characters";
        public const string KeypadButtonTypeRequired = "Keypad Button Type is Required";
        public const string KeypadButtonLevelIdRequired = "Level Id is Required";
        public const string KeypadButtonLevelIdInvalid = "Level Id is Invalid";
        public const string KeypadButtonShortDescLength = "Keypad Button Short Desc length can not be longer than 30 characters";
        public const string KeypadButtonDescLength = "Keypad Button Desc length can not be longer than 40 characters";
        public const string KeypadButtonColorLength = "Keypad Button Color length can not be longer than 10 characters";
        public const string KeypadButtonPasswordLength = "Keypad Button Password length can not be longer than 4 characters";
        public const string CategoryIdReq = "Category Id is Required";
        public const string SalesDiscountPercReq = "Percentage is required";
        #endregion

        #region StockAdjustHeader

        public const string StockAdjustLength = "Stock Adjust Reference length cannot be longer tha 10 characters";
        public const string StockAdjustHeaderNotFound = "Stock Adjust not found";
        public const string StockAdjustIncomplete = "Stock Adjust is incomplete";
        public const string ReferenceRequired = "Stock Adjust Reference is rquired";
        public const string StockAdjustReasonNotFound = "Stock Adjust Reason not found";
        public const string StockAdjustDetailItemNotFound = "Stock Adjust Line Item not found";
        public const string StockAdjustDeleted = "Stock Adjust Header Deleted";
        public const string StockAdjustDetailDeleted = "Stock Adjust Detail Deleted";
        public const string MustConfirmBatchTotal = "Must confirm batch total to allow posting";
        public const string StockAdjustmentDescriptionLength = "Description cannot be longer than 30 characters";
        #endregion

        #region StockTakeHeader

        public const string StockTakeLength = "Stock Take Desc length cannot be longer tha 40 characters";
        public const string StockTakeHeaderNotFound = "Stock Take records not found";
        public const string StockTakeDetailsNotFound = "Stock Take Details not found";
        public const string StockTakeIncomplete = "Stock Take is incomplete";
        public const string StoreTakeHeaderDuplicate = "Store Take Header already exists";
        public const string DescriptionRequired = "Stock Take Description is required";
        public const string StockTakeDetailItemNotFound = "Stock Take Line Item not found";
        public const string StockTakeDeleted = "Stock Take Header Deleted";
        public const string StockTakeDetailDeleted = "Stock Take Detail Deleted";
        public const string DescRequired = "Description is Required.";

        #endregion

        #region OutletSupplierSetting

        public const string OutletSupplierSettingDescLength = "OutletSupplierSetting Desc length cannot be longer than 30 characters";

        public const string OutletSupplierNotFound = "Outlet Supplier not found";
        public const string OutletSupplierId = "Outlet Supplier id is required.";
        public const string OutletSupplierDuplicate = "Outlet Supplier Already Exists";
        public const string InvalidSupplierId = "Invalid Supplier Id";
        public const string InvalidState = "Invalid State Id";
        public const string InvalidDivision = "Invalid Divisions Id";
        public const string InvalidOutletSuppier = "Invalid Data";
        public const string OutletSupplierSchedulesDuplicate = "Can not add duplicate values for Supplier.";

        #endregion

        #region StockOnHand

        public const string StockOnHandNotFound = "Stock on Hand not found";
        public const string ProductRangeInvalid = "product range not found";

        #endregion

        #region Competition Detail

        public const string CompetitionDetailCodeLength = "Code can't be longer than 15 characters";
        public const string CompetitionDetailDescLength = "Desc can't be longer than 30 characters";
        public const string CompetitionDetailMessageLength = "Message can't be larger than 100 characters";

        public const string CompetitionDeatilsIncomplete = "Competition Details Incomplete";
        public const string CompetitionDetailCompetitionTypeNotFound = "Competition type not found";
        public const string CompetitionCodeDuplicate = "Competition code is Duplicate";
        public const string ResetCycleNotFound = "Reset Cycle Not Found";
        public const string TriggerTypeNotFound = "Trigger type not found";
        public const string RewardTypeNotFound = "Reward type not found";
        public const string TriggerProductGroupNotFound = "Trigger Product group not found";
        public const string CompetitionNotFound = "Competition Data Not Found";

        #endregion
        #region JournalHeader

        //public const string JournalHeaderDateLength = "Journal Header Date can't be longer than 8 characters";
        //public const string JournalHeaderTimeLength = "Journal Header Time can't be longer than 6 characters";
        public const string JournalHeaderTypeLength = "Journal Header Type Can't be longer than 10 characters";
        #endregion

        #region JournalDetail

        public const string JournalDetailProductDescLength = "Journal Detail Product Desc can't be longer than 50 characters";
        public const string JournalDetailReferenceType = "Journal Detail Reference Type can't be longer than 25 characters";
        public const string JournalDetailReference = "Journal Detail Reference can't be longer than 64 characters";
        public const string JournalDetailTermRebateCode = "Journal Detail Term Rebate can't be longer than 64 characters";
        public const string JournalDetailScanRebateCode = "Journal Detail Scan Rebate can't be longer than 64 characters";
        public const string JournalDetailPurchaseRebateCode = "Journal Detail Purchase Rebate can't be longer than 64 characters";
        public const string JournalDetailDescLength = "Journal Detail Desc can't be longer than 50 characters";

        #endregion

        public const string ValidationErrorKey = "ValidationError";
        public const string GenericViewModelMandatoryMessage = "Please provide the proper information to proceed futher.";
        public const string ApplicationJsonContentType = "application/json";
        public const string GlobalUnhandledException = "Global Unhandled Exception: ";
        public const string InternalServerError = "Internal Server Error.";
        public const string SomeError = "Some error occured";
        public const string NotAcceptedOrCreated = "Data not accepted/created by server";
        public const string UnAuthorizeAccess = "You are not Authorized";


        #region Image Validation
        public const string ImageSizeError = "Max image size is 2MB";
        public const string ImageTypeError = "Image content type is invalid";
        public const string ImageNameError = "Image Name is required";
        #endregion

        #region
        public const string MaxGLADescLength = "Account Desc cannot be longer than 40 characters";
        public const string MaxGLASystemLength = "Account Desc cannot be longer than 10 characters";
        public const string MaxGLANumberLength = "Account Desc cannot be longer than 15 characters";
        public const string GLAccountDuplicate = "GL Account Already Exists.";
        public const string InvalidAccountSystem = "Invalid Account System.";
        public const string GLAccountNotExist = "GL Account not found";
        public const string GLAccountId = "GL Account Id is required";
        #endregion

        #region SupplierOrderSchedule

        public const string SupplierOrderingId = "Id is required.";
        public const string SupplierOrderingNotFound = "Supplier Ordering Schedule not found.";
        public const string SupplierOrderingDuplicate = "Supplier Ordering Schedule Already exists.";


        #endregion

        #region Xero Accounting
        public const string XeroAccountingDescLength = "Length can not be longer than 30 characters.";
        public const string XeroAccountingCodeLength = "Code can not be longer than 30 characters.";
        public const string XeroAccountingKeyLength = "Keys can not be longer than 50 characters.";

        public const string XeroAccNotFound = "Xero Account Not Found";
        public const string XeroAccDuplicate = "Xero Account With this Store already exists";
        public const string XeroAccId = "Xero Account Id required";
        public const string InvalidCode = "Code is invalid.";

        #endregion

        #region Transaction
        public const string TransactionTypeLength = "Transaction Type can't be longer than 10 characters";
        public const string TransactionReferenceLength = "Transaction Reference Can't be longer than 10 Caharacters";
        public const string TransactionDayLength = "Transaction Day can't be longer than 3 characters";
        public const string TransactionTenderLength = "Transaction Tender can't be longer than 15 characters";
        public const string TransactionGLAccountLength = "Transaction GL Account can't be longer than 15 characters";
        public const string TransactionFlagsLength = "Transaction Flags can't be longer than 400 characters";
        public const string TransactionReferenceType = "Transaction Reference Type can't be longer than 25 characters";
        public const string TransactionReferenceNumber = "Transaction Reference number can't be longer than 64 characters";
        public const string TransactionTermsRebateCode = "Transaction Terms Rebate Code can't be longer than 64 characters";
        public const string TransactionScanRebateCode = "Transaction Scan Rebate Code can't be longer than 64 characters";
        public const string TransactionPurchaseRebateCode = "Transaction Purchase Rebate Code can't be longer than 64 characters";
        #endregion

        #region Reporting
        public const string DataSourceIsEmpty = "Data source is empty";
        public const string DataSetNameIsEmpty = "Dataset name is empty";
        public const string NoDataFound = "No data found.";
        public const string ReportRequestModelIsEmpty = "Report request model is empty";
        public const string ReportNameReq = "Report Name is required.";
        public const string ReportNameNotFound = "Report not found.";
        public const string ReportCompulsorySelectionsRequired = "Compulsory selections required for {0} summary : {1}";

        public const string JournalSalesReportCompulsorySelectionsRequired = "Outlet selection needed for {0}";

        public const string LabelTypeRequired = "Label Type is Required";
        public const string PrintTypeRequired = "Print  Type is Required";
        public const string PriceLevelReq = "Price level is Invalid.";
        public const string LabelTypeUnavailable = "Label not Available";
        public const string PrintTypeUnavailable = "Print Type is not Available";
        public const string NoLabelAvailable = "No labels to print for this Store.";
        public const string InvalidPriceLevel = "Please enter price level between rane of 1 to 4.";
        public const string InvalidReprintDate = "Reprint Date is Required";
        public const string InvalidBatchDate = "Batch Print Date is Required";
        public const string InvalidSpecialDate = "Special Date From and Special Date Toare required.";
        public const string InvalidPromo = "Promotion Id is required.";
        public const string InvalidPromoId = "Invalid Promotion Id";
        public const string FormatRequired = "Invalid Format";

        #endregion

        #region POS Messages

        public const string MsgDescLength = "Description can not be longer than 50 characters.";
        public const string DayPartLength = "Day Parts can not be longer than 24.";
        public const string MinDayPartLength = "Fill all 24 Day parts.";
        public const string ReferenceLength = "Length can not be longer than 20.";
        public const string MessageRequired = "Message is required.";

        public const string POSNotFound = "Messages not found";
        public const string POSMsgId = "Message Id is required";
        public const string ReferenceIdRequired = "Reference Id is rquired";
        public const string InvalidDisplayId = "Invalid Display Type.";
        public const string InvalidReftype = "Invalid Reference Type.";
        public const string InvalidRefId = "Invalid Reference Id.";
        public const string InvalidProduct = "Invalid Product Number.";
        public const string InvalidCompetetion = "Invalid Competetion Code";
        public const string InvalidPrmotion = "Invalid Promotion Code";
        public const string InvalidCommodity = "Invalid Commodity Code";

        #endregion

        #region StockTRXSheet
        public const string DateRangeNotSelected = "Date Range is not selected";
        #endregion

        #region Invoice History
        public const string OrderPostedDateRangeNotSelected = "Order Posted Date Range Not Selected";
        public const string OrderInvoiceDateRangeNotSelected = "Order Invoice Date Range Not Selected";
        public const string DocumentTypeCREDIT = "Document Type CREDIT no longer supported, Credits must to be entered as Negative Invoices";
        public const string DocumentOrderTypeNotSelect = "Please select document type - ORDER";
        public const string NoOutletSupplierSetting = "No Outlet Supplier Setting Record? ";
        public const string DocumentNumberRequired = "Document number required";
        public const string DocumentDateRequired = "Document date required";
        public const string DocumentTotalsBalance = "Document Totals are out of balance";
        public const string DocumentHasBeenChanged = "This document has been changed by another user, your changes will be lost";
        public const string InvoiceNumberAlreadyExists = "This Invoice Number already exists for this Supplier";
        public const string DocumentTypeStatusSame = "Same it is docoument type and status.";
        public const string DocumentProdcutNotFound = "This Document has a zero Product number, Update not allowed.";
        public const string DocumentStatusRequired = "Document Status Required.";
        public const string DocumentTypeRequired = "Document Type Required.";
        public const string DocumentStatusNew = "Only New document status supported";
        public const string PDEFileNotExist = "PDE file not found in this path";
        public const string PDEPriceCheckFound = " PDE Price found.";
        public const string PDEPriceCheckNotFound = "No PDE Price difference found.";
        public const string PDEFileNotUpload = "PDE file not in correct format.";
        public const string PDEFileNotFound = "PDE file not found.";
        public const string OrderUpliftFactor = "Order uplift % shold be greater than 0 and less than 100";
        public const string SupplierScheduleMatching = "Can not find a matching Supplier Schedule.";
        public const string MarkOffineMessage = "Mark Offline";
        public const string OrderNotSent = "Order not sent.";
        #endregion

        #region RoleDefaultPermissions
        public const string ModuleMaxLength = "Module Name Length can't be longer than 100 Characters";
        public const string VerbMaxLength = "Verb Can't be longer than 6 Characters";
        #endregion

        #region MassPrice Update

        public const string InvalidOutletPass = "Invalid Outlet Password";
        public const string InvalidSysPass = "Invalid System Password";
        public const string InvalidSellGP = "Invalid Sell Price to GP%.";
        public const string ProductsUpdated = "Items Updated";

        #endregion

        #region ManualSale
        public const string ManualSaleCode = "Manual sale code cannot be longer than 30 characters";
        public const string ManualSaleDesc = "Manual sale desc cannot be longer than 80 characters";
        public const string ManualSaleCodeIsRequired = "Manual sale code is required";
        public const string ManualSaleDescIsRequired = "Manual sale desc is required";
        public const string ManualSaleCodeDuplicate = "Manual sale code already exists";
        public const string ManualSaleNotFound = "Manual sale not found";
        public const string ManualSaleItemNotFound = "Manual sale item not found";
        public const string ManualSaleInvalidQuantity = "Invalid Quantity";
        public const string ManualSaleInvalidAmount = "Invalid Amount";
        public const string ManualSaleInvalidPrice = "Invalid Price";
        public const string ManualSaleInvalidCost = "Invalid Cost";
        public const string ManualSalePriceLevelrequired = "Price Level required";
        public const string ManualSaleItemDuplicate = "Manual sale item already exists with combination manual sale id, product id and outlet id";
        #endregion

        #region Recipe

        public const string RecipeIdReq = "RecipeId is required.";
        public const string RecipeInvalid = "Recipe request is Invalid.";
        public const string RecipeNotExist = "Recipe does not exist.";

        #endregion

        #region Paths
        public const string PathIdReq = "Path is required.";
        public const string PathType = "Path type is Invalid.";
        public const string PathNotFound = "Paths records not found.";
        public const string DescriptionLength = "Paths description length cannot be longer than 100 characters.";
        public const string PathLength = "Path length cannot be longer than 200 characters.";
        public const string InvalidPathFile = "Please select a valid File.";
        public const string InvalidPathFileSize = "Please select a file of Size less than 2MB.";
        #endregion

        #region EPay
        public const string ItemLength = "Item length cannot be longer than 64 characters.";
        public const string Type = "Length cannot be longer than 5 characters.";
        public const string EpayLn = "Length cannot be longer than 50 characters.";
        public const string Code = "Length cannot be longer than 15 characters.";
        #endregion

        #region Rebate
        public const string ManufacturerId = "Manufacturer is Required";
        public const string InvalidProductId = "Invalid Product Id.";
        public const string RebateIdReq = "Rebate Id is required.";
        public const string RebateNotFound = "Rebate not Found.";
        public const string RebateInvalid = "Invalid Rebate Request.";
        public const string ZoneOrOutlet = "Please select anyone form Zone and Outlet.";
        public const string RebateItem = "You must add atleast one Product";
        #endregion

        #region CSCPeriod
        public const string Heading = "Heading cannot be longer than 25 characters.";
        #endregion

        #region Export
        public const string NoneSelected = "Select atleast one table to Export";

        public const string InvalidImport = "Selected File is not a valid CSV File.";
        public const string InvalidColumn = "Columns does not match in selected file.";

        public const string CSVNotFound = "CSV is required for import.";


        #endregion

        #region Host
        public const string Host = "Length cannot be longer than 30 characters.";
        #endregion

        #region Host
        //  public const string Host = "Length cannot be longer than 30 characters.";
        public const string HostSettingsNotFound = "HostSettings Not Found";
        public const string CostPriceZonesNotFound = "CostPriceZones Not Found";
        public const string CodeDuplicate = "This Code already exists";
        public const string WarehouseRequired = "Warehouse Id is required";
        public const string HostSettingRequired = "HostSettings Id is required";
        public const string HostFormatNotFound = "HostFormat Not Found";
        #endregion

        #region AccountTransaction
        public const string AccountTransactionTypeLength = "Transaction Type can't be longer than 10 characters";
        public const string AccountTransactionReferenceLength = "Transaction Reference Can't be longer than 10 Caharacters";
        public const string AccountTransactionDayLength = "Transaction Day can't be longer than 3 characters";
        public const string AccountTransactionTenderLength = "Transaction Tender can't be longer than 15 characters";
        public const string AccountTransactionGLAccountLength = "Transaction GL Account can't be longer than 15 characters";
        public const string AccountTransactionFlagsLength = "Transaction Flags can't be longer than 400 characters";
        public const string AccountTransactionReferenceType = "Transaction Reference Type can't be longer than 25 characters";
        public const string AccountTransactionReferenceNumber = "Transaction Reference number can't be longer than 64 characters";
        public const string AccountTransactionTermsRebateCode = "Transaction Terms Rebate Code can't be longer than 64 characters";
        public const string AccountTransactionScanRebateCode = "Transaction Scan Rebate Code can't be longer than 64 characters";
        public const string AccountTransactionPurchaseRebateCode = "Transaction Purchase Rebate Code can't be longer than 64 characters";
        #endregion

        #region AccountLoyalty
        public const string AccountLoyaltyAccountNumberLength = "Account Number Type can't be longer than 10 characters";
        public const string MaxAccountLoyaltyCompLength = "Comp Can't be longer than 15 Caharacters";
        public const string MaxAccountLoyaltyStatusLength = "Status can't be longer than 15 characters";
        public const string AccountLoyaltyComplianceLength = "Trigger Compliance can't be longer than 5000 characters";
        public const string AccountLoyaltyRewardInfoLength = "Reward Info can't be longer than 15 characters";
        public const string AccountLoyaltyReedeemInfoLength = "Redeem Info can't be longer than 400 characters";
        #endregion

        #region SystemControl
        public const string Name = "Name cannot be longer than 80 characters.";
        public const string KeySystem = "Length cannot be longer than 30 characters.";
        public const string SystemControlsNotFound = "System Controls records not found.";
        #endregion

        #region HostProcessings
        public const string Warehouse = "Not Found";
        public const string FilePathNotFond = "File Path is not found";
        public const string HostProcessingNotFond = "Host Processing Not Found";
        #endregion


        #region ReportSchedulers
        public const string NumberReq = "Number is Required";
        public const string UserIdReq = "Please select user.";
        public const string SchedulerExists = "Scheduler already exists";
        public const string SchedulerNotExists = "Scheduler not found";
        public const string ExportReq = "Export Format is required.";
        public const string SchedulerNotFound = "Export Format is required.";
        #endregion

        #region Item Sales reports
        public const string ItemSalesNotFound = "No sales found in the range selected";
        public const string ItemSalesDatesAreNotInMonthRange = "No Timeline Intervals(tbMONTH) could be calculated for this period.";
        public const string ItemSalesDatesAreNotInWeekRange = "No Timeline Intervals(tbWEEK) could be calculated for this period.";
        #endregion

        #region EDI Metcash Order
        public const string EDIMetcashSupplierCodeMissing = "Supplier code missing in order!";
        public const string EdDIMetcashSupplierItemMissing = "Order's supplier item missing!";
        public const string InvoiceOrderNotFound = "No order found in invoice";

        #endregion

        #region Ranking By Outlet report
        public const string RankingByOutletSalesNotFound = "No sales found in the range selected : {0} to {1}";
        #endregion

    }
}
