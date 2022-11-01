SELECT * FROm [Paths]


SELECT 
  CODE_KEY_ALP  [CODE]
, CODE_DESC     [Desc]
, CODE_ALP_1    [Initial Load File]
, CODE_ALP_2    [Weekly Load File]
, CODE_ALP_3    [Path]
, CODE_ALP_4    [Number factor]
, CODE_ALP_5    [Supplier]
, CODE_ALP_6    [Buy Promo Prefix]
, CODE_ALP_7    [Sell Promo Prefix]
, CODE_ALP_8    [Host Formate]
, CODE_NUM_1    [WareHouse] 
FROm [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].CODETBL 
WHERE CODE_Key_Type='PATH'