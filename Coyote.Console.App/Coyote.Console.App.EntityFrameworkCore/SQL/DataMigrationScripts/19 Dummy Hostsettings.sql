begin transaction
 begin try 

INSERt INTO HostSettings (
Code
,Description
,InitialLoadFileWeekly
,WeeklyFile
,FilePathID
,NumberFactor
,SupplierID
,WareHouseID
,BuyPromoPrefix
,SellPromoPrefix
,IsActive
,CreatedDate
,ModifiedDate
,CreatedBy
,ModifiedBy
,HostFormatID
)
SELECT 
 '99999' Code
,'NEW_UNALLOCATE' Description
, InitialLoadFileWeekly
, WeeklyFile
, FilePathID
, NumberFactor
, SupplierID
, WareHouseID
, BuyPromoPrefix
, SellPromoPrefix
, IsActive
, CreatedDate
, ModifiedDate
, CreatedBy
, ModifiedBy
, HostFormatID
 
FROm HostSettings WHERE Id=9
 --commit transaction

end try

begin catch
   SELECT
ERROR_NUMBER() AS ErrorNumber,
ERROR_SEVERITY() AS ErrorSeverity,
ERROR_STATE() AS ErrorState,
ERROR_PROCEDURE() AS ErrorProcedure,
ERROR_LINE() AS ErrorLine,
ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch

