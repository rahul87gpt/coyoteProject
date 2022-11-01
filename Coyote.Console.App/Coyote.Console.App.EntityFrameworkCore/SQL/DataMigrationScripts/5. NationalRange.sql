BEGIN
declare @RowCnt int
declare @MaxRows int
declare @ExecSql nvarchar(max)

select @RowCnt = 1
declare @nrtable Table (rownum int IDENTITY (1, 1) Primary key NOT NULL,RowData nvarchar(max))

insert @nrtable 
select cast('insert into [CoyoteConsoleApp_Staging].[dbo].[MasterListItems] ([ListId],[Code],[Name],[Status],[CreatedAt],[UpdatedAt],[CreatedById],[UpdatedById],[IsDeleted]) values (' as varchar(max)) +
cast('5' as varchar(max)) +','+
case when [NR_Number] is null then 'NULL' else ''''+ cast([NR_Number] as varchar(max))+'''' end +','+ 
case when REPLACE([NR_Desc],'''','''''') is null then 'NULL' else ''''+ cast(REPLACE([NR_Desc],'''','''''') as varchar(max))+'''' end+','+ 
cast('1' as varchar(max))+',cast('''+cast(getutcdate() as varchar(max)) + ''' as datetime)'+',cast('''+cast(getutcdate() as varchar(max)) +'''as datetime),'+'1'+','+'1'+','+cast('0' as varchar(max))+');'
  FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[NationalRangeTbl]
   

select @MaxRows=count(*) from @nrtable

begin transaction
 begin try 

while @RowCnt <= @MaxRows
begin
    select @ExecSql = ' ' + RowData + '' from @nrtable where rownum = @RowCnt 
    --print @ExecSql
    execute sp_executesql @ExecSql
    Select @RowCnt = @RowCnt + 1
end
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

END

--Select * FROm MasterListItems WHERE ListId=5