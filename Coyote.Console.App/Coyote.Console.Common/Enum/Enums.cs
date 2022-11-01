using System.ComponentModel;

namespace Coyote.Console.Common
{
    public enum PermissionEnum
    {
        None = 0,
        All = 1,
        Create = 2,
        Update = 3,
        GetAll = 4,
        GetById = 5,
        Delete = 6
    }
    public enum Status
    {
        None = 0,
        Active = 1,
        Pending = 2,
        Inactive = 3,
        Deleted = 4,
        Locked = 5,
    }

    public enum RoyaltyScale
    {
        Royalty = 1,
        Advertising = 2
    }
    public enum State
    {
        VIC,
        NSW,
        ACT,
        QLD,
        SA,
        WA,
        TAS,
        NT
    }
    public enum UOM
    {
        EA,
        CT
    }

    public enum GLAccountSystem
    {
        XERO = 1,
        SAGE = 2,
        MYOB = 3
    }

    //to change
    public enum ButtonSize
    {
        NOTVISIBLE = 0,
        NORMALSIZE = 1,
        DOUBLEWIDTH = 2,
        DOUBLEHEIGHT = 3,
        DOUBLEHEIGHTWIDTH = 4

    }

    public enum MessageRefType
    {
        [Description("PRODUCT")]
        PRODUCT = 1,
        [Description("PROMOTION")]
        PROMOTION = 2,
        [Description("COMPETITION")]
        COMPETETION = 3,
        [Description("CFD_DEFAULT")]
#pragma warning disable CA1707 // Identifiers should not contain underscores
        CFD_DEFAULT = 4,
#pragma warning restore CA1707 // Identifiers should not contain underscores
        [Description("COMMODITY")]
        COMMODITY = 5,
        [Description("RECIEPT")]
        RECIEPT = 6,
        [Description("REMINDER")]
        REMINDER = 7
    }

    public enum MessageDisplayType
    {
        CASHIER = 1,
        [Description("CFD_DEFAULT")]
#pragma warning disable CA1707 // Identifiers should not contain underscores
        CFD_DEFAULT = 2,
#pragma warning restore CA1707 // Identifiers should not contain underscores
        CUSTOMER = 3
    }

    public enum PromotionSource
    {
        MANUAL = 1,
        HOST = 2
    }

    public enum ActionPerformed
    {
        Create,
        Update,
        Get,
        Delete
    }
    public enum ReportType
    {
        Department,
        Supplier,
        Commodity,
        Group,
        Outlet,
        Category,
        Royalty,
        Advertising,
        Sales,
        Member,
        Stock,
        NoSales,
        ItemNoSales,
        LessthenXdays,
        ItemWithNegativeSOH,
        ItemWithZeroSOH,
        ItemWithSlowMovingStock
    }
    public enum IsRequired
    {
        True = 1,
        False = 0
    }

    public enum GeneralFieldFilter
    {
        None = 0,
        Equals = 1,
        GreaterThen = 2,
        EqualsGreaterThen = 3,
        LessThen = 4,
        EqualsLessThen = 5
    }
    public enum TillSyncType
    {
        SYNC = 1,
        DONE = 2
    }

    public enum ParentStatus
    {
        True = 1,
        False = 0
    }

    public enum PathType
    {
        EXPORT = 1,
        REPORTS = 2,
        USERPICS = 3,
        CASHIERPICS = 4,
        MEMBERPICS = 5,
        DEBTORPICS = 6,
        PRODUCTSPICS = 7,
        DEPARTMENTPICS = 8,
        GLEXPORT = 9,
        PDEPATH = 10,
        ORDPATH = 11,
        PROGRAMPATH = 12,
        WEBREPORTS = 13
    }

    public enum RebateType
    {
        [Description("TERMS REBATE")]
        TERMSREBATE,
        [Description("SCAN REBATE")]
        SCANREBATE,
        [Description("PURCHASE REBATE")]
        PURCHASEREBATE
    }

    public enum HostFormat
    {
        METCASH = 1,
        SPAR = 2
    }

    public enum CostPriceZoneType
    {
        Cost = 1,
        Price = 2
    }
    public enum TillJournal
    {
        Stopped = 0,
        Running = 1
    }
    public enum PriceRounding
    {
        None = 0,
        Fivecents = 1
    }

    public enum DefaultItemPricing
    {
        BestPrice = 4
    }

    public enum HostType
    {
        Weekly = 1,
        Initial = 2,
    }

    public enum DaysOfWeek
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thurday = 4,
        Friday = 5,
        Saturday = 6
    }

    public enum SchedulerInterval
    {
        Months = 1,
        Weeks = 2,
        Days = 3
    }
    public enum MasterListAccess
    {
        ReadWrite = 1,
        ReadOnly = 2,
        Hidden = 3
    }
    public enum PeriodicReportType
    {
        Daily,
        Weekly,
        Monthly
    }

    public enum ReportTryCount
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3
    }
}
