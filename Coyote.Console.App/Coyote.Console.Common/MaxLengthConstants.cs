using System.Security.Cryptography.X509Certificates;

namespace Coyote.Console.Common
{
    public static class MaxLengthConstants
    {
        #region User 

        public const int MinUserLength = 4;
        public const int MaxUserLength = 50;
        public const int MaxActivityLength = 50;
        public const int UserStatusLength = 8;
        public const int UserNameLength = 20;
        public const int UserPhoneLength = 15;
        public const int UserAddressLength = 30;
        public const int UserGenderLength = 13;
        public const int MaxUserNameLength = 100;

        #endregion

        #region ZoneType
        public const int MaxZoneTypeLength = 100;

        public const int MinZoneTypeLength = 30;
        #endregion

        #region Zone
        public const int MaxZoneLength = 80;
        public const int MinZoneLength = 30;
        #endregion

        #region StoreGroup
        public const int MinStoreGroupLength = 30;
        public const int MaxStoreGroupLength = 100;
        #endregion

        #region Role
        public const int MinRoleLength = 30;
        public const int MaxRoleLength = 50;
        #endregion

        #region User_Role
        public const int MinUserRoleLength = 30;
        public const int MaxUserRoleLength = 50;
        #endregion

        #region Store
        public const int MinStoreLength = 3;
        public const int MaxStoreLength = 255;
        public const int MaxEmailLength = 50;
        public const int StorePostCodeLength = 4;
        public const int StoreStatusLength = 10;
        public const int StorePhoneNoLength = 15;
        public const int StoreCodeLength = 30;
        public const int StoreDescLength = 40;
        public const int StoreLatitudeLength = 64;
        public const int StoreOpenHoursLength = 256;
        public const int StoreOpenTimeLength = 10;
        #endregion

        #region MasterList
        public const int MinListCodeLength = 50;
        public const int MaxListCodeLength = 100;
        #endregion

        #region MasterListItems
        public const int MinListItemCodeLength = 50;
        public const int MaxListItemCodeLength = 100;

        public const int ListItemCodeLength = 30;
        public const int ListItemNameLength = 80;
        #endregion

        #region ModuleActions 
        public const int MaxModuleActionNameLength = 100;
        #endregion

        #region EmailTemplates
        public const int MaxEmailTemplateLength = 1000;
        public const int MinEmailTemplateLength = 100;
        #endregion

        #region Department
        public const int MinDepartmentLength = 30;
        public const int MinDepartmentCodeLen = 3;
        public const int MaxDepartmentLength = 80;
        #endregion

        #region Commodity
        public const int MinCommodityLength = 30;
        public const int MaxCommodityLength = 80;
        #endregion

        #region Promotion
        public const int MaxPromotionCodeLength = 15;
        public const int MaxPromotionCodeLengthDB = 25;
        public const int MaxPromotionDescLength = 40;
        public const int MaxPromotionRptGroupLength = 15;
        public const int MaxPromotionAvailibilityLength = 500;
        //public const int MaxPromotionImagePathLength = 1000;
        #endregion

        #region PromotionProduct
        public const int MaxPromotionProductDescLength = 40;
        public const int MaxPromotionProductActionLength = 4;
        public const int MaxPromotionProductHostPromoTypeLength = 5;

        #endregion

        #region Tax
        public const int MinTaxCodeLength = 15;
        public const int MinTaxDescLength = 30;
        #endregion

        #region Warehouse
        public const int MinWarehouseCodeLength = 15;
        public const int MinWarehouseDescLength = 50;
        #endregion

        #region Product
        public const int MinProductDescLength = 50;
        public const int MinProductPosDescLength = 20;
        public const int MinProductFreightLength = 15;
        public const int MinProductSizeLength = 8;
        public const int MaxInfoLength = 50;
        public const int MinProductHostNumberLength = 15;
        public const int MinProductHostItemTypeLength = 1;
        public const int ProdReplicateLength = 15;
        #endregion

        #region SupplierProduct
        public const int MinSupplierProductDescLength = 50;
        public const int MaxSupplierItemCode = 15;
        #endregion
        #region Supplier
        public const int SuppCodeLength = 30;
        public const int SuppDescLength = 80;
        public const int SuppAddress1Length = 80;
        public const int SuppAddress2Length = 80;
        public const int SuppAddress3Length = 80;
        public const int SuppPhoneLength = 20;
        public const int SuppFaxLength = 20;
        public const int SuppEmailLength = 30;
        public const int SuppABNLength = 15;
        public const int SuppUpdateCostLength = 3;
        public const int SuppPromoCodeLength = 30;
        public const int SuppContactNameLength = 30;
        public const int SuppCostZoneLength = 30;
        public const int SuppGSTFreeItemCodeLength = 30;
        public const int SuppGSTFreeItemDescLength = 80;
        public const int SuppGSTInclItemCodeLength = 30;
        public const int SuppGSTInclItemDescLength = 80;
        public const int SuppXeroNameLength = 50;

        #endregion

        #region ZoneOutlet
        public const int MinZoneOutlet = 30;
        public const int MaxZoneOutlet = 80;
        #endregion

        #region Cashier
        public const int MaxCashierFirstNameLength = 20;
        public const int MaxCashierSurnameLength = 30;
        public const int MaxCashierAddressLength = 30;
        public const int MaxCashierPostcodeLength = 4;
        public const int MaxCashierPhoneLength = 15;
        public const int MaxCashierEmailLength = 50;
        public const int MaxCashierGenderLength = 6;
        public const int MaxCashierPasswordLength = 10;
        public const int MaxCashierWristBandIndLength = 3;
        public const int MaxCashierDispnameLength = 20;
        public const int MaxCashierLeftHandTillIndLength = 3;
        public const int MaxCashierFuelUserLength = 10;
        public const int MaxCashierFuelPassLength = 10;
        #endregion

        #region Keypad
        public const int MaxKeypadCodeLength = 50;
        public const int MaxKeypadDescLength = 50;
        #endregion

        #region Till
        public const int MaxTillCodeLength = 15;
        public const int MaxTillDescLength = 50;
        public const int MaxTillSerailNoLength = 15;
        public const int MaxTillVersionLength = 30;
        #endregion

        #region OrderHeader
        public const int MaxOrderRefLength = 15;
        public const int MaxOrderDeliveryNoLength = 15;
        public const int MaxOrderInvoiceNoLength = 15;
        #endregion

        #region APN
         public const int MaxAPNLength = 80;

        #endregion

        #region Xero Accounting
        public const int XeroAccountingCodeLength = 15;
        public const int XeroAccountingDescLength = 30;
        public const int XeroAccountingKeyLength = 50;
        #endregion

        #region OrderDetail
        public const int MaxOrderBuyPromoCodeLength = 15;
        public const int MaxOrderSalePromoCodeLength = 15;
        public const int MaxOrderSalePromoEndDateLength = 32;
        #endregion


        #region PrintLabelType
        public const int MaxPrintLabelTypeCodeLength = 30;
        public const int MaxPrintLabelDescCodeLength = 60;
        #endregion

        #region KeypadLevel
        public const int MaxKeypadLevelDescLength = 50;

        public const int MinLevelIndex = 1;
        public const int MaxLevelIndex = 100;

        #endregion

        #region KeypadButton
        public const int MaxKeypadButtonTypeLength = 30;
        public const int MaxKeypadButtonShortDescLength = 30;
        public const int MaxKeypadButtonDescLength = 50;
        public const int MaxKeypadButtonColorLength = 10;
        public const int MaxKeypadButtonPasswordLength = 4;

        public const int ButtonCountOnLevel = 36;

        public const int MinButtonIndex = 1;
        public const int MaxButtonIndex = 3600;
        #endregion

        #region Competition Detail
        public const int MaxCompetitionDetailCodeLength = 15;
        public const int MaxCompetitionDetailDescLength = 50;
        public const int MaxCompetitionDetailMessageLength = 100;

        #endregion

        #region StockAdjustmentHeader
        public const int MaxStockAdjustReferenceLength = 10;
        public const int MaxStockAdjustDescriptionLength = 30;
        #endregion

        #region OutletSupplierSetting
        public const int MaxOutletSupplierSettingDescLength = 50;
        #endregion

        #region JournalHeader
        ////YYYYMMDD
        //public const int MaxJournalHeaderDateLength = 8;
        ////HHMMSS
        //public const int MaxJournalHeaderTimeLength = 6;
        public const int MaxJournalTypeLength = 10;
        #endregion

        #region GLAccount
        public const int MaxGLADescLength = 40;
        public const int MaxGLASystemLength = 10;
        public const int MaxGLANumberLength = 15;
        #endregion
        #region JournalDetail

        public const int MaxJournalDetailProductDesc = 50;
        public const int MaxJournalDetailReferenceType = 25;
        public const int MaxJournalDetailReference = 64;
        public const int MaxJournalDetailTermRebateCode = 64;
        public const int MaxJournalDetailScanRebateCode = 64;
        public const int MaxJournalDetailPurchaseRebateCode = 64;
        public const int MaxJournalDetailDesc = 50;
        #endregion

        #region Transaction 
        public const int MaxTransactionTypeLength = 10;
        public const int MaxTransactionReferenceLength = 10;
        public const int MaxTransactionDay = 3;
        public const int MaxTransactionTenderLength = 15;
        public const int MaxTransactionGLAccountLength = 15;
        public const int MaxTransactionFlags = 400;
        public const int MaxTransactionReferenceType = 25;
        public const int MaxTransactionReferenceNumber = 64;
        public const int MaxTransactionTermsRebateCode = 64;
        public const int MaxTransactionScanRebateCode = 64;
        public const int MaxTransactionPurchaseRebateCode = 64;
        #endregion

        #region POS Mesage

        public const int ReferenceLength = 20;
        public const int DayPartLength = 24;
        public const int MsgDescLength = 50;
        public const int MsgPriority = 5;

        #endregion

        #region RolesDefaultPermissions
        public const int ModuleLength = 100;
        public const int VerbLength = 6;

        #endregion

        #region Paths
        public const int DescriptionLength = 50;
        public const int PathLength = 200;
        #endregion

        #region EPAY
        public const int ITEM = 64;
        public const int TypeCode = 5;
        public const int EPAYLn = 50;
        public const int ProductCode = 15;
        #endregion

        #region CSCPeriod
        public const int Heading = 25;
        #endregion

        #region Host
        public const int HostLength = 30;
        #endregion

        #region AccountTransaction 
        public const int MaxAccountTransactionTypeLength = 10;
        public const int MaxAccountTransactionReferenceLength = 10;
        public const int MaxAccountTransactionDay = 3;
        public const int MaxAccountTransactionTenderLength = 15;
        public const int MaxAccountTransactionGLAccountLength = 15;
        public const int MaxAccountTransactionFlags = 400;
        public const int MaxAccountTransactionReferenceType = 25;
        public const int MaxAccountTransactionReferenceNumber = 64;
        public const int MaxAccountTransactionTermsRebateCode = 64;
        public const int MaxAccountTransactionScanRebateCode = 64;
        public const int MaxAccountTransactionPurchaseRebateCode = 64;
        #endregion

        #region AccountLoyalty 
        public const int MaxAccountLoyaltyAccountNumberLength = 10;
        public const int MaxAccountLoyaltyCompLength = 15;
        public const int MaxAccountLoyaltyComplianceLength = 5000;
        #endregion

        #region
        public const int Name = 80;
        public const int KeySystem = 30;
        #endregion
    }
}
