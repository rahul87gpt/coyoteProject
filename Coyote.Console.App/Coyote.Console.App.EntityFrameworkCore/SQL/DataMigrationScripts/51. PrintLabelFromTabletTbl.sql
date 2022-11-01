--DELETE  FROM PrintLabelFromTabletTbl
INSERT INTO PrintLabelFromTabletTbl
SELECT * FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PrintLabelFromTabletTbl
