BEGIN

declare @RowCnt int
declare @MaxRows int
declare @ExecSql nvarchar(max)

select @RowCnt = 1
 
declare @Manufaturertable Table (rownum int IDENTITY (1, 1) Primary key NOT NULL,RowData nvarchar(max))

insert @Manufaturertable 
select cast('insert into [CoyoteConsoleApp_Staging].[dbo].[MasterListItems] ([ListId],[Code],[Name],[Col1],[Col2],[Num1],[Num2],[Num3],[Num4],[Num5],[Status],[CreatedAt],[UpdatedAt],[CreatedById],[UpdatedById],[IsDeleted]) values (' as varchar(max)) +
cast('3' as varchar(max)) +','+
case when [CODE_KEY_ALP] is null then 'NULL' else ''''+ cast([CODE_KEY_ALP] as varchar(max))+'''' end +','+ 
case when REPLACE([CODE_DESC],'''','''''') is null then 'NULL' else ''''+ cast(REPLACE([CODE_DESC],'''','''''') as varchar(max))+'''' end+','+ 
case when [CODE_ALP_1] is null then 'NULL' else ''''+ cast([CODE_ALP_1] as varchar(max))+'''' end+','+ 
case when [CODE_ALP_2] is null then 'NULL' else ''''+ cast([CODE_ALP_2] as varchar(max))+'''' end+','+ 
case when [CODE_NUM_1] is null then 'NULL' else ''''+ cast([CODE_NUM_1] as varchar(max))+'''' end+','+ 
case when [CODE_NUM_11] is null then 'NULL' else ''''+ cast([CODE_NUM_11] as varchar(max))+'''' end+','+ 
case when [CODE_NUM_12] is null then 'NULL' else ''''+ cast([CODE_NUM_12] as varchar(max))+'''' end+','+ 
case when [CODE_NUM_13] is null then 'NULL' else ''''+ cast([CODE_NUM_13] as varchar(max))+'''' end+','+ 
case when [CODE_NUM_14] is null then 'NULL' else ''''+ cast([CODE_NUM_14] as varchar(max))+'''' end+','+
cast('1' as varchar(max))+',cast('''+cast(getutcdate() as varchar(max)) + ''' as datetime) '+',cast('''+cast(getutcdate() as varchar(max)) +'''as datetime),'+'1'+','+'1'+','+cast('0' as varchar(max))+')'
  FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[CODETBL] where CODE_KEY_TYPE ='MANUFACTURER' 
   
select  @MaxRows=count(*) from @Manufaturertable 

begin transaction
 begin try 

while @RowCnt <= @MaxRows
begin
    select @ExecSql = '  ' + RowData + '' from @Manufaturertable where rownum = @RowCnt 
   -- print @ExecSql
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


--DECLARE @SQLQuery NVARCHAR(MAX)='',@Name VARCHAR(50)
--SELECT @Name='SAPNA''S'
--SET @SQLQuery =  @SQLQuery + ' SELECT CASE WHEN '''+REPLACE(@Name,'''','''''')+''' ='''+REPLACE(@Name,'''','''''')+''' THEN ''1'' ELSE '''+REPLACE(@Name,'''','''''')+''' END'
--EXECUTE( @SQLQuery)
--SELECT CASE WHEN 'SAPNA''S' <>'SAPNA''S' THEN '1' ELSE 'SAPNA''S' END

SELECT * FROM MasterListItems WHERE ListId=3