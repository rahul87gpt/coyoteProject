;WITH StoreLabelUpdateCTE 
AS
(
SELECT 
  OUTL_OUTLET
, new.Id
, LabelTypeShortId
, LabelTypePromoId
, LabelTypeShelfId
, (SELECT ID FROM PrintLabelType WHERE Code=OUTL_LABELTYPE_NORMAL) LabelTypeShelfId_Old
, (SELECT Code FROM PrintLabelType WHERE Code=OUTL_LABELTYPE_NORMAL)NormalLabelCode
, OUTL_LABELTYPE_NORMAL
, (SELECT ID FROM PrintLabelType WHERE Code=OUTL_LABELTYPE_PROMO) LabelTypePromoId_Old
, (SELECT Code FROM PrintLabelType WHERE Code=OUTL_LABELTYPE_PROMO)PromoLabelCode
, OUTL_LABELTYPE_PROMO
, (SELECT ID FROM PrintLabelType WHERE Code=OUTL_LABELTYPE_SHORT) LabelTypeShortId_Old
, (SELECT Code FROM PrintLabelType WHERE Code=OUTL_LABELTYPE_SHORT)ShortLabelCode
, OUTL_LABELTYPE_SHORT
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].OUTLTBL old
INNER JOIN Store new ON old.OUTL_OUTLET = new.Code
)
UPDATE StoreLabelUpdateCTE 
SET 
  LabelTypeShortId=LabelTypeShortId_Old
, LabelTypePromoId=LabelTypePromoId_Old
, LabelTypeShelfId=LabelTypeShelfId_Old