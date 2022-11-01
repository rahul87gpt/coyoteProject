DELETE FROM JournalDetail
DELETE FROM JournalHeader
DELETE FROM OrderDetail
DELETE FROm OrderHeader
DELETE FROm APN
DELETE FROM PromoMixmatchProduct
DELETE FROm PromoOfferProduct
DELETE FROm PromoSelling
DELETE FROm PromoBuying
DELETE FROM Recipe
DELETE FROm AccountTransaction
DELETE FROM StockAdjustDetail
DELETE FROM StockAdjustHeader
DELETE FROM StockTakeDetail
DELETE FROM StockTakeHeader
DELETE FROM OutletProduct
DELETE FROM PromoMixmatch
DELETE FROm PromoOffer
DELETE FROM PromoMemberOffer
DELETE FROM PromotionCompetition
DELETE FROM CompetitionTrigger
DELETE FROM PromotionCompetition
DELETE FROm CompetitionReward 
DELETE FROM PromotionCompetition
DELETE FROM CompetitionDetail
DELETE FROM Promotion
DELETE FROM SupplierProduct
DELETE FROM TillSync
DELETE FROM Till
DELETE FROM KeypadButton
DELETE FROm KeypadLevel
DELETE FROM Keypad
DELETE FROM ManualSaleItem
DELETE FROM Product
DELETE FROm COMMODITY
DELETE FROM Department
DELETE FROm OutletSupplierSetting
DELETE FROM CostPriceZones
DELETE FROM HostSettings
DELETE FROM Warehouse
DELETE FROM OutletSupplierSchedule
DELETE FROM GLAccount
DELETE FROm OrderAudit
DELETE FROm Supplier
DELETE FROM Cashier
DELETE FROM XeroAccount
DELETE FROM OutletRoyaltyScales
DELETE FROM OutletTradingHours
DELETE FROM ZoneOutlet
DELETE FROM BulkOrderFromTablet
DELETE FROM Paths
DELETE FROm OutletBudget
DELETE FROM Store
DELETE FROM PrintLabelType
DELETE FROM PrintLabelFromTabletTbl
DELETE FROM StoreGroup
DELETE FROM Messages

DELETE FROM ModuleActions
DELETE FROm ManualSale
DELETE FROm RebateDetails
DELETE FROm RebateOutlets
DELETE FROM RebateHeader
DELETE FROM [Transaction]
DELETE FROM MasterListItems
DELETE FROM UserLog
DELETE FROM Tax 
DELETE FROM MEMBER
DELETE FROM AccountLoyalty
DELETE FROM ACCOUNT
DELETE FROm [User] WHERE Id<>1
DELETE FROm UserRole WHERE UserId<>1
DELETE FROm Role WHERE ID <>1
--SELECT * FROm SystemControls -- Not Deleted as discussded Manoj 
--HOSTUPD No data found
--EPAY No data found
--CSCPERIOD No Data found
--BulkPrintLabelFromTabletTbl No Data Found
--BestBuySupplier No Data Found
--SELECT * FROm [User]


