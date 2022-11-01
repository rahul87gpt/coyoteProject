Begin Transaction 
Begin Try
--DBCC CHECKIDENT('PromoMemberOffer', RESEED, 0)
insert into PromoMixmatch (PromotionId,QTY1,AMT1,DiscPcnt1,PriceLevel1,Qty2,Amt2,DiscPcnt2,PriceLevel2,CumulativeOffer,Status,isdeleted,createdAt,updatedAt,createdbyId,updatedbyid,[Group])

select 
  p.Id PromotionId
--, o.Code_key_Alp
, CODE_ALP_1
, REPLACE(CODE_ALP_2,'$','') as AMT1
, CODE_ALP_3 as DISCPCNT1
, CODE_ALP_4 as PL1
, CODE_ALP_5
, REPLACE(CODE_ALP_6,'$','') as AMT2
, CODE_ALP_7 as DISCPCNT2
, CODE_ALP_8 as PL2
, (case when CODE_ALP_9 ='N' then 0 else 1 end) CO
, 1 status
, 0 Deleted
, GETDATE() c
, GETDATE() u
, 1 cb
, 1 ub 
, ISNULL((SELECT PRMH_MIX_OFF_COL FROm [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PRMHTBL WHERE PRMH_PROM_CODE=o.Code_key_Alp),' ') AS grp
 FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[Codetbl]  o
 LEFT OUTER JOIN Promotion p on p.Code=o.Code_key_Alp
 WHERE Code_key_type = 'MIXMATCH'
 AND p.id is NOT null
  --AND  o.Code_key_Alp IS NOT NULL

--commit transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 

  --SELECT * FROm Promotion WHERE PromotionTypeId=19848 AND Status=1
  --Select * from MasterListItems Where ListId=10








 ------------------------------Below MIXMatch Offer Not found in the Promotion Header table--------------
 --SELECT * FROm PRMHTBL WHERE PRMH_PROM_CODE IN (
--SELECT *FROM [Codetbl] WHERE Code_key_type = 'MIXMATCH' AND CODE_KEY_ALP IN (
--'1.25KRKDEEPSPRW',
--'11132',
--'123123456123456',
--'1694396',
--'700MOTHER',
--'700NUWATER',
--'700ROCKSTAR',
--'700SCHW',
--'700SCHW1.25L',
--'701_28',
--'701_ARIZONA',
--'701_DARE',
--'701_ICE',
--'701_KITKAT3',
--'701_MOTHER250',
--'701_POWERADE',
--'701_REHAB',
--'701_SKITTLES',
--'701_TICTAC',
--'701_TIMTAMS',
--'701_TWINK',
--'701_USDRINKS',
--'701_V250',
--'701241',
--'701COKE600',
--'701MOTHER',
--'701MTFRAN600',
--'702_BEN&JERRY',
--'702_BUNDABERG',
--'702_CAD',
--'702_COKE1.25LT',
--'702_COKE6002',
--'702_ICE',
--'702_LIPTON',
--'702_MILK2FOR',
--'702_MILK2FOR7',
--'702_MUSASHI',
--'702_NIPPYS',
--'702_NUTRIWATER',
--'702_POWERADE1L',
--'702_PUMP',
--'702_SCHWEPPES',
--'702_TAPOUT',
--'702_USADRINKS',
--'702_V500',
--'702_VIVALOE',
--'702_WHITTAKER',
--'702003',
--'702004',
--'703_2',
--'7040COKE',
--'704CAD',
--'704CLIFF',
--'704ICE',
--'704MIKE',
--'704MOTHER',
--'704MT',
--'704REDBULL',
--'704YOG',
--'705-KIRKS1.25',
--'705-MINI',
--'705-REDSKINS',
--'705-WINGS',
--'705_',
--'7054WING',
--'705CHICKEN',
--'706BLUEBERRYBAL',
--'706CONFEC',
--'706MAXIMUS',
--'706MAXIMUS1LT',
--'706SAMBOY',
--'706SCHWEPPES',
--'706STARBURST',
--'706WATER1.5LT',
--'706YFZBLISSBALL',
--'707_',
--'707_1.25LWEST',
--'707_28SPECIAL',
--'707_COKE',
--'707_DARE',
--'707_ICE3',
--'707_KWSCHW',
--'707_NUTRI',
--'707_REP',
--'707_VIVA',
--'70703KARIN_',
--'7071.25L_',
--'707BEN&JERRY_',
--'707COKE18PACK_',
--'707COOL_',
--'707EGG_',
--'707FIJI_',
--'707FIJI500_',
--'707ICE_',
--'707ICEBREAK_',
--'707ICEBREAK2_',
--'707IMP_',
--'707IMPJPS_',
--'707KISSRANGE',
--'707NUTRI',
--'707NUTRIENT',
--'707NUTRIENTDE_',
--'707PRINGLES_',
--'707V500ML_',
--'707VALLEY_',
--'707VGREEN500ML_',
--'708_',
--'708_SCHWEPPES',
--'708_SMITH',
--'708_TEASER',
--'708BBERG_',
--'708CBELLA1LT_',
--'708COKE600ML_',
--'708CREAM',
--'708GAYTIME',
--'708ICE_',
--'708JPSRYO_',
--'708MAGNUM_',
--'708MENTOS_',
--'708MTFRANKLIN6_',
--'708NUT',
--'708POWERADE1LT_',
--'708PUMP750ML_',
--'708REDBULL473_',
--'708SCHW600ML_',
--'708SCHWEPPES_',
--'709-3XICE',
--'709-SOUTHCAPE',
--'709-V500',
--'709_TIMTAM',
--'709CHR',
--'709CK',
--'709COKE2017',
--'709EASTER',
--'709EASTER1',
--'709ICE',
--'709MT',
--'709PUMP',
--'710-TARRAGINDI',
--'710_',
--'710_CADBURY',
--'710_COCACOCA',
--'710_COCACOLA',
--'710_COOLRI1.5L',
--'710_DARE',
--'710_DARE2',
--'710_GATORIDE1L',
--'710_KIRK1.25',
--'710_LINALT',
--'710_LYNX',
--'710_PARTYBAG',
--'710_REDBULL',
--'710_REDROCK',
--'710_SAMOSA',
--'710_SCHW',
--'710_SMITHS',
--'71001',
--'71001MAXIMUS',
--'71001SMITH',
--'711-KIRK1.25',
--'7111LPOWERADE',
--'711DRINK',
--'711GATRO',
--'711GOLDENCIRC',
--'711GREATAUSSI',
--'713-ALLENS120',
--'713-COKE2L',
--'713-GATRIDGE',
--'713-ICE4KG',
--'713-MARSBITES',
--'713-SBRAND',
--'713-SCHW1.25',
--'713-SCHWEP125',
--'713-WRIGLEY',
--'713_BBQCOKE',
--'713_BNJ_458',
--'713_BOND25S',
--'713_BOND40S',
--'713_CHOICE',
--'713_COOLRIDGE',
--'713_JPS26S',
--'713_JPSTOB',
--'713_LBEACHMOM',
--'713_LONGBEACH',
--'713_P/JACKSON',
--'713_P/STY',
--'713_P15COCA600',
--'713_PETERSTU',
--'713_SPAR46',
--'713_STAW',
--'713_SWEETCHILL',
--'713_TNCC',
--'713_TOMATOES1K',
--'713_V500ML$7.',
--'713_VDRINKS',
--'713_WINFEILDTW',
--'713_WINFIELDTW',
--'713_WINGDINGS',
--'713_WINYTWIN',
--'715-DIMMEE',
--'715TENDERS',
--'716-SHAPE',
--'716_',
--'716_COKE2LTR',
--'716_COOLRIDGE',
--'716BARISTABROS',
--'716CADCHOC',
--'716COKE',
--'716FUZE',
--'716JTS',
--'716MONSTER',
--'716POWERADE',
--'716PRING',
--'716SCHW',
--'716SMARTWATER',
--'716TIMTAM',
--'716VITAMINWATE',
--'717_COCA375ML',
--'717_SAMBOY',
--'717_STARBURST',
--'71KIRK',
--'720-001',
--'720_G/CIRC',
--'720_G/CRIC_2L',
--'720_GSTUNA',
--'720_PRINGLES',
--'720_SMITHS',
--'724_BNJ',
--'724_SOUTH',
--'724_WING',
--'72410',
--'72411',
--'72413',
--'724CHILLI',
--'724CHOOK',
--'724GARLIC',
--'724MNP100',
--'724MNP250',
--'724MNPFF250',
--'724WING',
--'725-JTS',
--'725-MAXIMUS1L',
--'725-PUMP',
--'725_CANS',
--'725_CLWT1',
--'725_DARE',
--'725_ICE',
--'725_MOTHER',
--'725_NUTRIENT',
--'725_REDBULL',
--'72503',
--'725BALLS',
--'725BEN&JERRY_',
--'725CANS',
--'725CHICKEN',
--'725COKE',
--'725COOL',
--'725COOLRIDGE',
--'725ICE',
--'725JTS320ML',
--'725MTFRANKLIN',
--'725NESTMEDBR',
--'725NIGHTWATER',
--'725NUTREIN',
--'725P/ADE1L',
--'725PUMP',
--'725ROCKSTAR',
--'725SCH1.25',
--'725SSCH1.25',
--'725STARBRUST',
--'725V350ML',
--'725W/DINGS',
--'725WAT',
--'725YGT',
--'72708OREO',
--'7271',
--'728-TRU-BLU',
--'728_COOL600',
--'7282LCOKE',
--'728AMERICAN',
--'728BOWLSCHOOKS',
--'728CADCRISP',
--'728DORITOS',
--'728HUGGIES',
--'728ICEMOOL',
--'728KIRKS2F5',
--'728MEDBAR',
--'728SCHWEPP',
--'728SOOTH',
--'728THINS',
--'728TIMTAM',
--'728WATERCHIP',
--'728WIMMERS',
--'72902',
--'72909',
--'729440MLCANS',
--'729ALLENS',
--'729ANG',
--'729COKE18PACK',
--'729COKE500',
--'729COKE600ML1',
--'729HOTFOOD',
--'729HOTFOOD2',
--'729ICE',
--'729JCS',
--'729KETTLE',
--'729KETTLE2',
--'729LION',
--'729MARS',
--'729MED',
--'729MENTOS2',
--'729MENTOS3',
--'729MENTOS4',
--'729MENTOS5',
--'729MONSTER',
--'729MOTHER',
--'729MTFRA',
--'729MTFRAN',
--'729MTFRAN01',
--'729MTFRAN02',
--'729NUWATER',
--'729OAK',
--'729OVI',
--'729PAULS',
--'729PET',
--'729POWER',
--'729REDBULL',
--'729ROBINA29',
--'729ROC',
--'729SCH1.25L',
--'729SHAPES',
--'729SMITHS',
--'729SOUTHPORT',
--'729STAR',
--'729TICTAC',
--'729TNCC',
--'729TOWER',
--'729TRUBLU',
--'729USADRINKS',
--'729VBOTTLES',
--'729VPROMO',
--'729WATER',
--'730-MCCAIN',
--'730-REGFRSTBITE',
--'730_BEN&JERRY',
--'7313FOR$4',
--'731ARNOTTS',
--'731DIMSIMS',
--'731GC',
--'731HERO',
--'731ICE',
--'731OREO',
--'731POTATO',
--'731PRCC',
--'731TIMTAM',
--'733-ICE',
--'733-PIE',
--'736-GO01',
--'736-GO02',
--'736-GO03',
--'736-GO04',
--'738',
--'738-G/CIRC',
--'738_',
--'738_CADMED',
--'738_CCA390',
--'738_CCA600ML',
--'738_COOLWATER',
--'738_MOTHER',
--'738_MTFRANK1.5',
--'738_MTFRNK',
--'738_MTFRNK1.5',
--'738_NIGHTOWL1.5',
--'738_NIGHTOWL1L',
--'738_NUWATER600',
--'738_SCHW',
--'738_V500ML',
--'73801JERIN',
--'738CHICDRM',
--'738CHICGARLIC',
--'738CHICTENDE',
--'738KATRINA',
--'738MT/FRANK600',
--'738PORKRIB',
--'739-1.25RANGE',
--'739-28ENRGY250M',
--'739-ARD-DUO-TIN',
--'739-ARN-CRM-CHP',
--'739-ARN-PLAIN',
--'739-ARN-SHP/JAT',
--'739-ARN-WTR-CRK',
--'739-BALST-D/WSH',
--'739-BKDBEAN420',
--'739-CAD-BISC125',
--'739-CAD-BLKS220',
--'739-CAD-MED',
--'739-CAD/NES-BLK',
--'739-CAD/NES-MED',
--'739-CHEEKIES72G',
--'739-CHUPA-CHUP',
--'739-CHWY+1LCOOL',
--'739-CMB-CAD-COK',
--'739-CMB-PEP+SBA',
--'739-COKE-CAN375',
--'739-COOL600ML',
--'739-CRISPELLO',
--'739-CUPASOUP-2S',
--'739-D/SPR-1.25L',
--'739-DF-T/CRM150',
--'739-DORITOS',
--'739-E/MAC-BOWL',
--'739-F/FARE-CREM',
--'739-FAB-L/PD500',
--'739-FAN-NDL-CUP',
--'739-G/C-BRD750G',
--'739-G/CIRC-2L',
--'739-G/CIRC-DRK1',
--'739-G/MAX_BRD',
--'739-HNZ-BNS420G',
--'739-HNZ-SOUP420',
--'739-HNZSOUP535G',
--'739-ICE-BAG-5KG',
--'739-ICE5KG',
--'739-J/WEST-TUNA',
--'739-KELL-CEREAL',
--'739-KETTLE185G',
--'739-KFT-DIPS185',
--'739-KIRKS-2L',
--'739-KIRKS1.25L',
--'739-KRK/DSPR/MT',
--'739-LA-GINA-TIN',
--'739-LA-GINA400G',
--'739-LISTRN-250M',
--'739-LUCOZADE300',
--'739-MAG-CUP-NDL',
--'739-MALT-BUNNY',
--'739-MARS-MED',
--'739-MAXIMUS-1L',
--'739-MCC-STK750G',
--'739-MF-SCE-500M',
--'739-MIGORENG85G',
--'739-MILKYWAY2PK',
--'739-MISSN/CHIP',
--'739-MOGU-DRNK',
--'739-MUSASHI-P30',
--'739-NAB-250GM',
--'739-NAB-RITZ',
--'739-NES-BLOCKS',
--'739-NONG/SHIM',
--'739-OREO-150G',
--'739-PALM-D/W500',
--'739-PALM-H/WSH',
--'739-PAPA/G-PIZZ',
--'739-PEARS-SPOOO',
--'739-PEDIGR-700G',
--'739-PEP-SCH1.25',
--'739-PEPSI-CANS',
--'739-PURINA-85GM',
--'739-PWRADE-600M',
--'739-REAL/S-500M',
--'739-REAL/STK250',
--'739-REDBULL-250',
--'739-REX-DEOD150',
--'739-S/SILK-200M',
--'739-S/TOM400G',
--'739-SAKATA-100G',
--'739-SANTA-M/WTR',
--'739-SBA-175GM',
--'739-SBA175G-RNG',
--'739-SCH/PEP-CAN',
--'739-SCHW-1.25L',
--'739-SCHW-2L',
--'739-SCHW/PEP2L',
--'739-SHAPES+JATZ',
--'739-SMITHS+DOR',
--'739-SMITHS-175G',
--'739-SMITHS-45G',
--'739-SMT/DOR/XCR',
--'739-SORBNT6PK',
--'739-SPC-BKD-BNS',
--'739-STRBRST-BAG',
--'739-SUMN-NDLCUP',
--'739-SUNRICE-CUP',
--'739-SUPER-GROCR',
--'739-SURF-500GM',
--'739-TETLEY-TBAG',
--'739-TICTAC',
--'739-TIM-TAM-90G',
--'739-TOBLERONE',
--'739-TUNA-POT100',
--'739-TWIST-170GM',
--'739-TWIST-90GM',
--'739-TWISTIS170G',
--'739-TWNG-TEA10S',
--'739-USA-BARS',
--'739-V250ML',
--'739-VAALIA-160G',
--'739-W/W-CKE/MIX',
--'739-W/W-FLOUR1K',
--'739-WHISKAS-400',
--'739-WHISKAS400G',
--'739-WONKA-NERDS',
--'739-WTR-PLS600',
--'739-XMAS-CLEARN',
--'739-ZAF-PAST500',
--'739-ZFRLLI-500G',
--'739-ZOOSH-DRSIN',
--'739_ARN_CHOC',
--'739_KINDER43G',
--'739_KIRKS_2L',
--'739_LINDT_100G',
--'739_MIGORENT5PK',
--'739_NES_BLOCKS',
--'739_PRNGL_150G',
--'739_TIM_TAMS',
--'739_TUNA95GM',
--'739ALLENS-L/BAG',
--'739ARN-PLN/TT90',
--'739ARN-TT90/PLN',
--'739CMB-COKE+BOS',
--'739CMB-PEP+SMIT',
--'739COLG-T/P-3PL',
--'739CONT-P/S-85G',
--'739COOL-WTR1.5L',
--'739D/F-YGT-T/CR',
--'739DUO-L/PDR500',
--'739EDG-VEGE-TIN',
--'739HANS-HAM100G',
--'739HANS-STCK15G',
--'739L/ZADE-380ML',
--'739LUCO-SPRT500',
--'739M/M-BUNNY35G',
--'739MCC-CHIPS1KG',
--'739NES-L/SAVERS',
--'739NUTELLA-220G',
--'739P1312-NESMED',
--'739RAZ-KNEHI-2S',
--'739WRIG-5-GUM',
--'739XXX-MINTS45G',
--'741-2FORCOKE',
--'741_ICE',
--'743_CHUPA5FOR2',
--'743_V250',
--'744-OWLWTR',
--'744-SHAPES',
--'744ESPLANADE',
--'744JERIN001',
--'744JERIN002',
--'745_',
--'745_2MILK-TEST',
--'745_COKE1.25',
--'745_KIRK1.25',
--'745_MILK2FOR',
--'746-GO01',
--'746-GO02',
--'746-GO03',
--'746-GO04',
--'747CAD200G',
--'747CHRISTMAS',
--'747CHUP',
--'747COCO',
--'747COKE',
--'747DA',
--'747DARE',
--'747DAREE',
--'747FREDDO',
--'747FRUCO',
--'747GATORADE',
--'747ICE',
--'747MALTBUNNY',
--'747MO',
--'747MOTHER',
--'747MT',
--'747NESTLE',
--'747NIGHTOWL',
--'747PS',
--'747PUMP',
--'747REDBULL',
--'747STRAW',
--'747SUIMIN',
--'747TIMTAM',
--'747TNCC',
--'747V350ML',
--'747YOG',
--'749-SCHWMM',
--'7494N20PARTY',
--'749AMERICANDRIN',
--'749ARNCROWNS',
--'749ARNDELTA',
--'749ARNMONTE',
--'749ARNROYALS',
--'749ARNVARIETY',
--'749ASIANDRINKS',
--'749BUBBLY',
--'749BULLASPLITS',
--'749CADBLOCK',
--'749CHINCHIN',
--'749CLASSIC',
--'749COKECANS',
--'749COOL1LT',
--'749DIP',
--'749DUO',
--'749FIJIWATER',
--'749FRESHUP',
--'749GOLDMAX',
--'749GRAINWAVES',
--'749GREENPOWER',
--'749HANSSTRIKER',
--'749HEINZSOUP',
--'749HEINZSOUP2',
--'749ICEBREAK',
--'749ILLUMINATED',
--'749INDOMIEMIGOR',
--'749KIRKS',
--'749KIWI',
--'749LOL',
--'749M&M''SEASTER',
--'749MANGOS',
--'749MIGORENG',
--'749OATSEXPRESS',
--'749OSHA',
--'749REDBOX',
--'749REDBULL250',
--'749SARALEE',
--'749SHAPES',
--'749SMITHS',
--'749SMITHS175G',
--'749SMITHS45G',
--'749SUIMIN',
--'749TNCC',
--'749TWSMITE',
--'749UP&GO',
--'749VIVALOE',
--'749WHISKAS',
--'750-10-002',
--'750-10-01',
--'750-10-03',
--'750-10-06',
--'750-10-07',
--'750-10-08',
--'750-10-09',
--'750-10-10',
--'750-10-11',
--'750-10-12',
--'750-10-13',
--'750-10-14',
--'750-10-16',
--'750-10-18',
--'750-10-19',
--'750-10-20',
--'750-10-21',
--'750-10-22',
--'750-10-23',
--'750-10-24',
--'750-10-25',
--'750-10-26',
--'750-10-27',
--'750-10-28',
--'750-10-29',
--'750-11-03',
--'750-11-04',
--'750-11-05',
--'750-11-07',
--'750-11-08',
--'750-11-09',
--'750-11-11',
--'750-11-12',
--'750-11-13',
--'750-11-14',
--'750-11-15',
--'750-11-16',
--'750-11-17',
--'750-11-19',
--'750-11-21',
--'750-11-22',
--'750-11-23',
--'750-11-24',
--'750-11-25',
--'750-11-26',
--'750-11-27',
--'750-11-28',
--'750-11-29',
--'750-11-30',
--'750-11-31',
--'750-11-33',
--'750-11-34',
--'750-11-35',
--'750-11-36',
--'750-11-37',
--'750-11-38',
--'750-11-39',
--'750-11-40',
--'750-11-41',
--'750-11-42',
--'750-11-43',
--'750-11-44',
--'750-11-45',
--'750-11-48',
--'750-11-55',
--'750-12-1',
--'750-12-10',
--'750-12-11',
--'750-12-12',
--'750-12-13',
--'750-12-14',
--'750-12-15',
--'750-12-16',
--'750-12-17',
--'750-12-18',
--'750-12-19',
--'750-12-2',
--'750-12-20',
--'750-12-21',
--'750-12-4',
--'750-12-5',
--'750-12-6',
--'750-12-7',
--'750-12-8',
--'750-12-9',
--'750-122',
--'750-1809-02',
--'750-1809-03',
--'750-1809-04',
--'750-1809-05',
--'750-1809-06',
--'750-1809-07',
--'750-1809-08',
--'750-1809-09',
--'750-1809-10',
--'750-1809-11',
--'750-1809-12',
--'750-1809-13',
--'750-1809-14',
--'750-1809-15',
--'750-1809-16',
--'750-1809-17',
--'750-1809-18',
--'750-1809-19',
--'750-1809-20',
--'750-1809-21',
--'750-1809-22',
--'750-1809-23',
--'750-1809-24',
--'750-1809-25',
--'750-1809-26',
--'750-1809-27',
--'750-1809-28',
--'750-1809-29',
--'750-1809-30',
--'750-1809-31',
--'750-1809-32',
--'750-1809-33',
--'750-1809-34',
--'750-1809-35',
--'750-1809-36',
--'750-1809-37',
--'750-1809-38',
--'750_CAD',
--'750_NUTRIENT2',
--'750CHICKEN',
--'751-COKE450ML',
--'751-KALIJERARIC',
--'751-PARATHA751',
--'751-PRAN',
--'751-PUFFRICE',
--'751_B&J',
--'751_CALAMARI',
--'751_FROZVEG',
--'751_G',
--'751_GABBAR',
--'751_GABBASCHWE',
--'751_HASHBROWN',
--'751_KET/CCS',
--'751_MTFRANK600M',
--'751_NIWATER',
--'751_NUWATER1.5L',
--'751_PRAWNPOTA',
--'751CHICTENDER',
--'751CUPJELLY',
--'751DESIPARATH',
--'751GARLIBALLS',
--'751LOLIPOPSX',
--'751MANGOBAR',
--'751MUKHIX2',
--'751NUWATER',
--'751PURI',
--'751SAM/SIN/SP',
--'751STR2L',
--'751WHLOLLIPO',
--'752-KIRKS1.25L',
--'752-SOX',
--'752_3XICE',
--'752_BEN&JERRY',
--'752_KIRKS1.25L',
--'752_MTFRNK1.5',
--'75214',
--'752CAD',
--'752CHUPACHUP',
--'752CTCKNS',
--'752FRUIT',
--'752WATER+TOAST',
--'753',
--'753-1.25KIRKS',
--'753-1.5MF',
--'753-600COCACO',
--'753-600MF',
--'753-750PUMP',
--'753-COCOWATER',
--'753-DORITOS170',
--'753-GS',
--'753-MOUNTFRANK',
--'753-MT/FRANK',
--'753-NESTEALEMO',
--'753-SCHWP',
--'753-SGYOUGURT',
--'753-UP&GO',
--'753-YOUGURTMAN',
--'753-YOUGURTPAS',
--'753_DORITOS',
--'75301',
--'75301-PROTEIN-B',
--'75301-SCHWEPPES',
--'75301WESTEND',
--'75312OZCOFFEE',
--'75316OZCOFFEE',
--'753CHUPA',
--'753COKE',
--'753SCHWEPPES',
--'754_',
--'754_2CHKN',
--'754_BLU',
--'754_COKE2L',
--'754_COKE390',
--'754_COKE600',
--'754_DARE',
--'754_EASTER',
--'754_EASTEREGG',
--'754_FRUIT&VEG',
--'754_ICECREAM',
--'754_KIRKS',
--'754_MTFRANK',
--'754_NATNOOD',
--'754_PUMP',
--'754_REDBULL',
--'754_REDBULL473',
--'754_SCH',
--'754_SCH600',
--'754_SMITHS',
--'754_TAKEAWAY',
--'754_TRU',
--'754_TRUBLU',
--'754_V',
--'754_WATER',
--'754BACON&EGG',
--'755_SCHDEAL',
--'756-SUMMER26',
--'756-SUMMER30',
--'756-SUMMER34',
--'756-SUMMER35',
--'756440ML',
--'756ALLEN',
--'756BAR',
--'756BREAK',
--'756BREAKA',
--'756CADB135',
--'756CADKIN',
--'756CCA1',
--'756CHUP',
--'756CLASSIC',
--'756COCA',
--'756COK250',
--'756COKE',
--'756COKE125',
--'756COLA',
--'756CONN',
--'756COOL',
--'756COOL600',
--'756COOL750',
--'756DA500ML',
--'756DA750',
--'756DF2L',
--'756DRUM',
--'756ECLIP',
--'756EXTRA',
--'756GAT',
--'756GAY',
--'756GLAC',
--'756ICE',
--'756KINDER',
--'756KIRKS',
--'756LIFE',
--'756LIP',
--'756MA',
--'756MAG',
--'756MAG2',
--'756MARSK',
--'756MAXIBON',
--'756MENPUR',
--'756MOTH500',
--'756MT15L',
--'756MTFRA',
--'756MTSPARK',
--'756NESKIN',
--'756NESMED',
--'756NOBY',
--'756NUTKIN',
--'756OAK600',
--'756OBINA8',
--'756OVI',
--'756PADE',
--'756POWRE',
--'756PRING53',
--'756PUM125',
--'756PUMP750ML',
--'756RED250',
--'756REDBULL',
--'756RIDGE',
--'756ROBINA1',
--'756ROBINA10',
--'756ROBINA21',
--'756ROBINA24',
--'756ROBINA32',
--'756ROBINA4',
--'756ROBINA5',
--'756ROCK',
--'756RRDGW',
--'756SCHW125',
--'756SCHW600ML',
--'756SMITHS175',
--'756SMITHS45',
--'756SMITHS90',
--'756TNCC',
--'756UNCLE',
--'756UPGO',
--'756V250',
--'756V350',
--'756V355',
--'756V500',
--'757-786',
--'757-CADBLOCKS',
--'757-CONFCBAG',
--'757-ICE',
--'757-MARSHANG',
--'757-MARSHANGSEL',
--'757-REDBULL',
--'757-SCHWEPPES',
--'757-SMITHSCH',
--'757-STARBURST',
--'757-V500MLRAN',
--'757_',
--'757_2LTCOKE',
--'757_BARIS500ML',
--'757_CADBURYEGG',
--'757_KIRK2FOR$5',
--'757_MOTHER250ML',
--'757_PUMP750ML',
--'757_SCHW1.25',
--'757_STARBURST',
--'757_V2FOR$5',
--'757_V500ML',
--'757CADBITESI',
--'757CADBURY',
--'757GCIRCLE',
--'757SCHWEPPES',
--'758_',
--'758_2CHKNS$12',
--'758_COKE375',
--'758_SCH1.25LTR',
--'758FRANTELLE',
--'760-CHIPS',
--'760_',
--'760_CADBIGBLOCK',
--'760_COOLWATER',
--'760_COOLWATER12',
--'760_JUICE',
--'760_MIKE',
--'760_OATS',
--'760_SCHWE1.25L',
--'760_SHAPES',
--'760_SPEC',
--'760_TIMTAM',
--'760CAD',
--'760KAT',
--'760SUIMIN',
--'762_',
--'762_CADBLOCKS',
--'762_CHUPA',
--'762_COKE2LT',
--'762_FUZE',
--'762_G/CIRCL1LT',
--'762_G/IRC1LT',
--'762_GARLIC',
--'762_MAXIMUS',
--'762_MILK3FOR$8',
--'762_SCH1.25',
--'762_SCH1.25LT',
--'762_SCHW',
--'762_V5OOML',
--'762600ML',
--'762AUSTSOAP',
--'762BREAD',
--'762BUFFALO',
--'762CHUPACHUP',
--'762COKE390ML',
--'762COKE500ML',
--'762DONUT',
--'762GLAV/WATER',
--'762KINEGG',
--'762KIRKS1.25',
--'762KIRKS1.25LT',
--'762MARSBUY1',
--'762MONSTER',
--'762MOTHER5OOML',
--'762SMT/WATER',
--'762TIMTAM',
--'762WIGS',
--'763-ICEX3',
--'764-FROZEN',
--'764_CADCHOC',
--'764_CADMEDIUM',
--'764_FRANK',
--'764_L/SAVERS',
--'764_ROB04',
--'764_TIMTAM',
--'76401KIRKS',
--'764MM',
--'764ZYMIL',
--'765_BUFFALO',
--'765_CHUPA4FOR2',
--'765_COFCOOK',
--'765_GARLICBALLS',
--'765_GBALLS',
--'765_GHOSTDROP',
--'765_MEALDEAL',
--'765_SAMOSA',
--'765_SWEETCHILLI',
--'765_WINGDINGS',
--'765FISH',
--'766_',
--'766_BLUERIBBON',
--'766_CAD',
--'766_CHIP',
--'766_CHUP3FOR2',
--'766_COKE2016',
--'766_COKE2L',
--'766_COKE2LT',
--'766_FREDDO',
--'766_GATORADE',
--'766_MONSTER',
--'766_MTFRANKLIN',
--'766_PUMPALLYEAR',
--'766_QUICKSCH',
--'766_REDBULL',
--'766_SCHW',
--'766_SCHWEPPES',
--'766_SCHWP3',
--'766_STARBURST',
--'766_TIMTAM',
--'766_TIMTAM2',
--'766_TNCC',
--'766CAD',
--'766CHICKENPROM',
--'766CHICKGARLIC',
--'766CHILLISTRIPS',
--'766CHKKIRKS',
--'766DEVILWINGS',
--'766NU',
--'766RR',
--'767-DARE750ML',
--'767-NOWLWATR600',
--'767-P2DORITOS',
--'767_CCA/SCHW',
--'767_HEROP10',
--'767BURGERRINGS',
--'767DARE500ML',
--'767DORITSCHS170',
--'767DRINKS1.25L',
--'767FRUIT3X2',
--'767GATLEMONLIME',
--'767KIRK1.25L',
--'767KIRKMIX',
--'767KIRKS1.25',
--'767KITKATDRK45G',
--'767MAXIMUSLEMON',
--'767P2DRINKS1.25',
--'767TOWER',
--'767TSTCFFEE',
--'767WATER+TOA',
--'768',
--'768-BOOSTTWIN',
--'768_',
--'768_02',
--'768_09',
--'768_10',
--'768_18',
--'768_222',
--'768_224',
--'768_24',
--'768_COKE375ML',
--'768_DAREESPRES',
--'768_ICE',
--'768_KISSBAG',
--'768_NP15',
--'768_PUMP',
--'768_RB250',
--'768_SHAPES',
--'768_SOURSTRAPS',
--'768_SWEETNSOU',
--'768_TGB',
--'768_TICTAC',
--'768_WRAPDEAL',
--'768_YOWIE',
--'768COFF&MUFFIN',
--'768COKE600ML',
--'768ICE',
--'768MT/FRANK',
--'768SODAWTR',
--'769',
--'769-2�4KGICE',
--'769-2FORCOKE',
--'769-BNJ',
--'769-CADMED',
--'769-CONNOIS',
--'769-DORITOS175',
--'769-G/CIR1L',
--'769-NU/WAT600M',
--'769-PWRADEX2',
--'769-REDBULL473M',
--'769-SAMB175',
--'769-SCHWEPPES',
--'769-SHAPES',
--'769-STARBURST',
--'769-T/TOP700GM',
--'7692XCOKE600ML',
--'769FRUIT',
--'769KIRKS',
--'769MONSTER',
--'771-2XICE',
--'771-CADMARVELL',
--'771-CHIPS',
--'771-SUMMER-D1',
--'771-SUMMER-D3',
--'771_',
--'772-BBQCHICKEN',
--'772-BNJ',
--'772-CADCHOC',
--'772-CHIPS',
--'772-COOLWATER',
--'772-G/CIRCJCE',
--'772-M&M',
--'772-PETERS',
--'772-SCHWEPPES',
--'772-SHAPES',
--'772_',
--'772_BNJN',
--'772_JUICE',
--'772_PRINGLE',
--'772_TIMTAM',
--'772_TROPICE',
--'772MTF',
--'772MTF1.5L',
--'773',
--'773-2�4KGICE',
--'773-2�CONSSIOR',
--'773-2COKE600M',
--'773-BFASTBARS',
--'773-BNJ',
--'773-CADBLOCK',
--'773-CADMED',
--'773-DORITOS175',
--'773-G/CIR1L',
--'773-MONSTERS',
--'773-MT/FRANK',
--'773-NU/WAT600M',
--'773-P/ADE600ML',
--'773-REDBULL473M',
--'773-SAMB175',
--'773-SCHMAX',
--'773-SCHWEPPES',
--'773-STARBURST',
--'773_HIPROTIN',
--'773_KIT3MINT',
--'7732XCOKE600ML',
--'775_2FORSCHWEPP',
--'775_3FOR5',
--'775_KITKAT',
--'776',
--'776-SCHW1.25',
--'776_SMITHSFLAVS',
--'7761LWATER',
--'776ALLENS',
--'776B/BERG',
--'776CAD',
--'776CADBLOCK',
--'776CADMEDIUM',
--'776COKE600',
--'776COOLWATER',
--'776EAGLEBOYS',
--'776FLAKE',
--'776GUM',
--'776ICEBREAK',
--'776ICEOFFER',
--'776MARSMED',
--'776MOTHER',
--'776MT600ML',
--'776NESMEDIUM',
--'776NIGHHT1.5L',
--'776NIGHTOWL600',
--'776PIECESCHIC',
--'776PUMP',
--'776QUESTBAR',
--'776TIMTAM',
--'776VE',
--'776WATER',
--'776YGT',
--'778_RICK004',
--'778RICK034',
--'779_EASTEREGG',
--'779_MIXNMATCH',
--'779_MON&SHAPE',
--'779_TALLYHO',
--'779_TUNA95G',
--'779MARLBORO',
--'780-KIRKS1.25',
--'780_1',
--'780HOTDOG/COKE',
--'781-ALLENS',
--'781-HER01',
--'781-HER02.1',
--'781-HER03.1',
--'781-HER04.1',
--'781-HER05.1',
--'781-HER06',
--'781-HER06.1',
--'781-HER07.1',
--'781-PIZZAS',
--'78101LIS',
--'7810SCHW',
--'782-TRUBLU',
--'782BREAD',
--'782CADBURYBAR',
--'782COKECAN',
--'782CONTINENTAL',
--'782FAB',
--'782GATORADE',
--'782ICE',
--'782INDOMIE',
--'782KIRKS',
--'782NUTIRIENTWAT',
--'782SANTAELVES',
--'782SCHWEPPES',
--'782SHASTACANS',
--'782TAKEAWAY',
--'782WONKATWIST',
--'783-AMERICANA',
--'783-AVO2',
--'783-BOOST',
--'783-CHIKO',
--'783-GARLICBALL',
--'783-GREATAUSSIE',
--'783-MAXIMUS-1LT',
--'783-SCHWE-1.25',
--'783-SWEETCHILL',
--'783-W26-X1',
--'783-W26-X2',
--'783_BBQWINGS',
--'783_CHKTENDER',
--'783_FREDDO',
--'783_FREDO2$1',
--'783_GARLIBALLS',
--'783_KIRK1.25',
--'783_MANGO',
--'783_SCHWEPPES',
--'783_SPREE',
--'783_SSORBTWIN',
--'7832LTR',
--'7832TRCOKE',
--'783ALLEN',
--'783AVOCADO',
--'783AVOCADOS',
--'783CHIKDINDEAL',
--'783COFFWRAP',
--'783CORN',
--'783HANK',
--'783MANG244.',
--'783MARSTOWER',
--'783MEDIUMBARX2',
--'783MT600',
--'783TNC',
--'784-CAD100',
--'784-FREDDO20',
--'784_',
--'784ARNOTTS',
--'784AVOCADOS',
--'784B/GOLDTEABAG',
--'784B/NUTSNAP',
--'784BRANPLUS',
--'784CADBURY',
--'784CAULIFLOWER',
--'784CEREALS',
--'784CHICKEN',
--'784CHICKENCAN',
--'784COCOWATER',
--'784COUNTRYLADLE',
--'784CUPASOUP',
--'784CUSTARDCREAM',
--'784DIMSIM',
--'784DORITOSMITHS',
--'784FANTASTIC',
--'784FIJIWATER',
--'784FOIL',
--'784GATORADE',
--'784GLADWRAP',
--'784GREATAUSSIE',
--'784HEINZBEANZ',
--'784ICE',
--'784KITKAT',
--'784KIWIFRUIT',
--'784MANGOES',
--'784MINIDIMSIM',
--'784PIEFRUIT',
--'784PINEAPPLE',
--'784POWERADE',
--'784REALSTOCKVEG',
--'784ROCKMELON',
--'784ROCKSTAR',
--'784SCHWEPPES',
--'784STRAWBERRIES',
--'784STRAWBERRY',
--'784SUNSILK',
--'784SWEETCORN',
--'784TETLEY',
--'784TUNA',
--'784V500',
--'784VITAWEAT',
--'785-MTFRNK1.5',
--'785_KWSCHW',
--'785_SCHWEDSPL',
--'785_TIMTAM',
--'786-2XICE',
--'786_',
--'786_COCACOLA',
--'786_COKE2LTR',
--'786_DEERAGUN',
--'786_MTFRANKLIN',
--'786_WINDDING',
--'787-B/GOLDNAP',
--'787-BBQCHOOK',
--'787-CANNON',
--'787-ICEBREAK500',
--'787-MTFRANK1.5',
--'787-PUMP',
--'787_C/HILL',
--'787_GREEN',
--'787_KITKAT',
--'787_MANGO',
--'787_SNICKERS',
--'787_TABASCO',
--'787_V500ML',
--'787787',
--'787ALOEVERA95',
--'787ARNOTT',
--'787BEL',
--'787CHICKENTOIL',
--'787COK1',
--'787COK2',
--'787COK3',
--'787COOLRIDGE',
--'787COOLWATER1LT',
--'787FBLSBACON',
--'787HANGOVER',
--'787ICE',
--'787LIPTON',
--'787MARK01',
--'787MARK02',
--'787MARK03',
--'787MARK04',
--'787MARK08',
--'787MARK19',
--'787MARK20',
--'787MARK21',
--'787MARK22',
--'787SCH245',
--'787TABASCO',
--'788-COKE1.25',
--'788_G/CRIC_2L',
--'788_MDAY2',
--'788_MDAY3',
--'788_MDAY4',
--'788_MDAY6',
--'788_PANCAKEMIX',
--'788_R&PINE',
--'788_TIMTAM',
--'788ARNCREAM',
--'788CHIPS',
--'788COMMWATER',
--'788CWATER',
--'788KAMAL007',
--'788KP',
--'788LOLLYCUP',
--'788MISSION',
--'788P12',
--'788PELLE',
--'788PRING',
--'788RICK001',
--'788RICK003',
--'788RICK004',
--'788RICK005',
--'788RICK006',
--'788RICK007',
--'788RICK008',
--'788RICK009',
--'788RICK011',
--'788RICK015',
--'788RICK016',
--'788RICK017',
--'788RICK021',
--'788RICK022',
--'788RICK024',
--'788RICK025',
--'788RICK026',
--'788RICK027',
--'788RICK028',
--'788RICK030',
--'788RICK034',
--'788RICK035',
--'788RICK037',
--'788RICK038',
--'788RICK039',
--'788RICK040',
--'788RICK041',
--'788RICK042',
--'788RICK043',
--'788RICK044',
--'788RICK045',
--'788RICK046',
--'788RICK047',
--'788RICK049',
--'788RICK050',
--'788RICK052',
--'788RICK053',
--'788RICK054',
--'788RICK055',
--'788RICK056',
--'788RICK059',
--'788RICK060',
--'788RICK062',
--'788RICK063',
--'788RICK48',
--'788RICKSCHWCAT',
--'789_COOLRIDGE',
--'789BALLS',
--'789CHILLI',
--'789DIMSIM',
--'789DING',
--'789ONIONS',
--'789POTATO',
--'789SAUSAGE',
--'789WINGS',
--'790-AMAZOO',
--'790-BBQ',
--'790-BBREAD',
--'790-CREAMCHEESE',
--'790-DDALE',
--'790-DFCLSC500',
--'790-ENERGIZE',
--'790-FRENCHFRIES',
--'790-GCIRC',
--'790-HANS',
--'790-ICINGMIX',
--'790-MILKYWAY',
--'790-MILO',
--'790-ROCKBYFARM',
--'790-TEDDY',
--'790-TEDDYMOO',
--'790-THINLY',
--'790-TIMTAM',
--'790-TITAN',
--'790-VGLASS',
--'790_',
--'790_BNJ458',
--'790_COKE600ML',
--'790_GAT',
--'790_ICE',
--'790_ICEBAG',
--'790_JT',
--'790_MARK03',
--'790_MARK06',
--'790_MARK12',
--'790_MARK13',
--'790_MARK14',
--'790_MF600ML',
--'790_MT/FRANKLIN',
--'790_MUSCLEMILK',
--'790_POWERADE',
--'790_PUMP',
--'790_QNT',
--'790_TICTAC',
--'790_TIMTAM',
--'790_YFZPBALL',
--'790ALMONDBFT',
--'790MARK02',
--'790MARK04',
--'790MARK06',
--'790MARK07',
--'790MARK08',
--'790SCHW',
--'790SCHWSODA',
--'792-3XDIMEE',
--'792-AUST',
--'792-BBQ',
--'792-CHILSTRIP',
--'792-CHUPA',
--'792-COOKIES',
--'792-DIMEES',
--'792-DIMMEES',
--'792-DIMSIM',
--'792-F&V',
--'792-GBALLS',
--'792-ICE',
--'792-KINDER',
--'792-SMITHSFLAV',
--'792-SSTENDERS',
--'792-XMASELVES',
--'792-XMASSANTA',
--'792-XMASSMAN',
--'792_CHUPA',
--'792_CROQ',
--'792_DIMEES',
--'792_POPCORNSPL',
--'792_SCH1.25',
--'792_STRIPS',
--'792_TENDERS',
--'792CHICWINGS',
--'792COCOCOLLECT',
--'792COOLRIDGE',
--'792COOLWATER',
--'792DONUT',
--'792GARLICBALLS',
--'792TIMTAM',
--'792YOOSH55ML',
--'793-CLWT1',
--'793-MAXIMUS',
--'793-MOTHER',
--'793-OZSPR1.5',
--'793_',
--'793_001',
--'793_007',
--'793_NOWL750ML',
--'793_NU600ML',
--'793_NU600MLSP',
--'79301',
--'79301BEN&JERRY',
--'793C600ML',
--'793COOL',
--'793COOLRIDGE',
--'793DARE',
--'793F&V',
--'793GATORADE',
--'793GATRODE',
--'793JTS320ML',
--'793JTS510ML',
--'793MONSTER500ML',
--'793MTFRANK',
--'793NIGHTWATER',
--'793NIGHWATER',
--'793NU600ML',
--'793NUPURE1.5L',
--'793OZSPRING',
--'793REDBULL',
--'793SSCH1.25',
--'793SSS1.25',
--'793STARB',
--'793V350',
--'793V350ML',
--'793WAT',
--'793WATER',
--'794',
--'794-JTS',
--'794-MTFRANKLIN',
--'794_',
--'794_2INT',
--'794_600MLWATE',
--'794_MOUNTFRANK',
--'794_NUPUREWAT',
--'795_',
--'796-ALLENS',
--'796-CHIPS',
--'796-COKEENRG',
--'796-FUZE',
--'796-MT/FR',
--'796-PUMP',
--'796-SCHWSODAWTR',
--'796-TENDER',
--'796-WATER',
--'796RIBS',
--'797-001',
--'797_',
--'797CHEEZEL',
--'797CHIKO',
--'797COCACOLA',
--'797COKE375',
--'797DIM/G',
--'797FISH',
--'797KIRKS',
--'797PEPSI',
--'797PRING',
--'797REDROCK',
--'797TWIBRING',
--'797WATER',
--'798-CKNSTRIPS',
--'798-GARLICBALLS',
--'798-ICE',
--'798-KINGDER20G',
--'798-OREO45',
--'798-PUMP750',
--'798-WDING',
--'798_QUEST/QNT',
--'7982XCOKE$6',
--'798SWTCHILLI',
--'799-QNTBAR',
--'799_COCOBELLA',
--'799_COKE600ML',
--'799_COOLRIDGE',
--'799_GATORADE',
--'799_MANGO',
--'799_MAXIMUS1',
--'799_MMMM',
--'799_POWERADE',
--'799_POWERADE1L',
--'799_PUMP2FOR6',
--'799_SCHW',
--'799_WATER',
--'799CBALL',
--'799GROVE',
--'799KITKAT',
--'857-171102',
--'998-2X5',
--'998-CCA250',
--'998-COKE2X',
--'998-COKE4X',
--'998-COKE600',
--'998-DATETEST',
--'998-WATER',
--'998_FSP16-MKTKI',
--'998_ICE',
--'CADMEDGABBA',
--'CHIKOROLL',
--'COFFEE',
--'CRAB-17',
--'CTPHASE8',
--'CTPHASE8-C',
--'CTPHASE8-D',
--'DEEPSPRING',
--'EASTER',
--'FIH',
--'FSO-055',
--'FSO-056',
--'FSO-057',
--'FSO-058',
--'FSO-060',
--'FSO-061',
--'FSO-062',
--'FSO-063',
--'FSO-064',
--'FSO-065',
--'FSO16-MKT',
--'KAT',
--'KATRINA',
--'KISES',
--'M2SCORE',
--'MAT-12',
--'MAXIBON',
--'MAXXCLEARANCE',
--'MM1',
--'MM2',
--'NY',
--'OREO731',
--'P/ADE725',
--'P1210-BULLSMIT',
--'P1210-FCBMBAR',
--'P1210-MARSCOKE',
--'P1210-SINGLES',
--'P1211-HYPERSNAK',
--'P1213-ALLENTNCC',
--'P1213-CADKGBAR',
--'P1213-COKE1.25',
--'P1213-COKE2LT',
--'P1213-DRGRCRCH',
--'P1213-GAYTIME',
--'P1213-GCIRC1LT',
--'P1213-GONAT',
--'P1213-GONATPROT',
--'P1213-GRAINWAVE',
--'P1213-MARS175',
--'P1213-MIZONE750',
--'P1213-MOTHER500',
--'P1213-MTFRANK',
--'P1213-NDARE750',
--'P1213-NOAK',
--'P1213-NTWTR575',
--'P1213-OWLWTR600',
--'P1213-PWRADE600',
--'P1213-QBREAKA',
--'P1213-QDARE750',
--'P1213-RBULL473',
--'P1213-SCHW1.25',
--'P1213-SCHW600',
--'P1213-VOAK',
--'P13-COKE1.25',
--'P1301-BEROCCA',
--'P1301-COKE1.25',
--'P1301-COKE2L',
--'P1301-COKE600',
--'P1301-CRIDGE1L',
--'P1301-DRUMSTICK',
--'P1301-FRESHUP',
--'P1301-GATORADE',
--'P1301-GCIRCT1L',
--'P1301-GGTIME',
--'P1301-IBREAK500',
--'P1301-MARS',
--'P1301-MASTER',
--'P1301-MAXIMUS',
--'P1301-MTFRNK600',
--'P1301-NESTEA500',
--'P1301-NESTLESML',
--'P1301-RBULL250',
--'P1301-RRD185',
--'P1301-SBRAND',
--'P1301-TNCC',
--'P1301-V500',
--'P1302-CADMED',
--'P1302-CBAG',
--'P1302-COKE125',
--'P1302-COKE500',
--'P1302-CRIDGE600',
--'P1302-DARE',
--'P1302-EXTRA',
--'P1302-GCIRCLE1L',
--'P1302-KETTLE90',
--'P1302-MARS2PK',
--'P1302-MASTER',
--'P1302-MCGGX6',
--'P1302-MIZONE750',
--'P1302-MOTHER500',
--'P1302-MTFRANK15',
--'P1302-NESTLEMED',
--'P1302-NUTWATER',
--'P1302-POWERADE',
--'P1302-PUMP750',
--'P1302-SCHWEP125',
--'P1302-SCHWEP600',
--'P1302-SMITHS90',
--'P1302-SMTHSAK',
--'P1302-SUNISLIM',
--'P1302-V350',
--'P1302-WEIS',
--'P1303-COKE1.25',
--'P1303-COKE600',
--'P1303-CRIDGE1.5',
--'P1303-DF600NSW',
--'P1303-DF600QLD',
--'P1303-DF600VIC',
--'P1303-FRESHUP',
--'P1303-GGTIME',
--'P1303-GRADE600',
--'P1303-ICEBRK750',
--'P1303-KETTLE185',
--'P1303-KINGCAD',
--'P1303-MARSEGG',
--'P1303-MEDMARS',
--'P1303-MFRNKSPRK',
--'P1303-NESTEA500',
--'P1303-SCHW440',
--'P1303-SMITH175',
--'P1303-TNCC',
--'P1303-UPGO350',
--'P1303-V250',
--'P1303-V500',
--'P1304-ALLENS120',
--'P1304-BREAK600',
--'P1304-CADKNG',
--'P1304-COKE1.25',
--'P1304-COKE2L',
--'P1304-CRIDGE1L',
--'P1304-DARE500',
--'P1304-EXTRAE',
--'P1304-MAGNUM',
--'P1304-MFRNK1LT',
--'P1304-NCHIP150',
--'P1304-NUTRIWTR',
--'P1304-OAK600',
--'P1304-PWRADE600',
--'P1304-RBULL473',
--'P1304-RBULLSPEC',
--'P1304-RIBENA500',
--'P1304-RRD175',
--'P1304-RSTAR355',
--'P1304-SCHW600',
--'P1304-SFREDDO',
--'P1304-THINS90',
--'P1304-V350',
--'P1305-AEROKAT',
--'P1305-ANGLLB',
--'P1305-BREAK750',
--'P1305-CADKING',
--'P1305-CLASSIC-N',
--'P1305-COKE1.25',
--'P1305-COKE2LT',
--'P1305-COKE600',
--'P1305-CRIDGE',
--'P1305-DFCLASSIC',
--'P1305-DJUICE',
--'P1305-DRUMSTICK',
--'P1305-FREDDO',
--'P1305-GATORADE',
--'P1305-KETTLE185',
--'P1305-MARMED',
--'P1305-MONST500',
--'P1305-MOTHER250',
--'P1305-NESMED',
--'P1305-NESTEA',
--'P1305-PUMP750',
--'P1305-REDBULL',
--'P1305-SCHWSMIT',
--'P1305-TNCC',
--'P1306-CADMED',
--'P1306-COKE1.25',
--'P1306-CRIDGE1.5',
--'P1306-DARFUIC',
--'P1306-MAR2PAK',
--'P1306-MOTHER500',
--'P1306-NESMED',
--'P1306-PARMALAT',
--'P1306-POWER600',
--'P1306-REDBULL',
--'P1306-SCHW440',
--'P1306-SMITH90',
--'P1306-SMTDORGRN',
--'P1307-5GUM',
--'P1307-ALLENS',
--'P1307-CHIPS',
--'P1307-COKE1.25',
--'P1307-COKE2LT',
--'P1307-COKE600',
--'P1307-CRIDGE',
--'P1307-DFARM',
--'P1307-DJUICE',
--'P1307-GATOR',
--'P1307-GGTIMCAL',
--'P1307-GGTIME',
--'P1307-ICEBREAK',
--'P1307-MONSTER',
--'P1307-MTFRNK',
--'P1307-NESTGLACE',
--'P1307-NESTSMALL',
--'P1307-RRD',
--'P1307-SMLFRED',
--'P1307-SMTCRLCRN',
--'P1307-UPNGO',
--'P1307-V250ML',
--'P1307-V500ML',
--'P1308-BIGMUNION',
--'P1308-BREAK750',
--'P1308-CADMED',
--'P1308-CHIMPWURL',
--'P1308-CHIP175',
--'P1308-COKE1.25',
--'P1308-COKE500',
--'P1308-CRIDGE600',
--'P1308-FRANK500',
--'P1308-KETTLE',
--'P1308-KSIZE',
--'P1308-MARS2PAK',
--'P1308-MOTHER500',
--'P1308-MWAY25GM',
--'P1308-NESTGLACE',
--'P1308-PWRADE',
--'P1308-SCHW440',
--'P1308-SCHW600',
--'P1308-SCHWDORIT',
--'P1308-SCHWEPPE',
--'P1308-TNC',
--'P1308-V250',
--'P1308-V500',
--'P1309-COKE1.25',
--'P1309-COKE2',
--'P1309-COKE600',
--'P1309-CRIDGE',
--'P1309-DARE500',
--'P1309-DARE700',
--'P1309-DJUICE',
--'P1309-FREDLRG',
--'P1309-GRADE',
--'P1309-MAGNUM',
--'P1309-NESTEA',
--'P1309-OAK',
--'P1309-PQ1',
--'P1309-PUMP',
--'P1309-RRD',
--'P1309-SBRAND',
--'P1309-SCHW',
--'P1309-SMITH45',
--'P1309-V350',
--'P1309-V500',
--'P1310-ALLENS',
--'P1310-BHOUSE',
--'P1310-CADMED',
--'P1310-COKE1.25',
--'P1310-FRANKLIN',
--'P1310-ICEBREAK',
--'P1310-MARS2PAK',
--'P1310-POWERADE',
--'P1310-REDBULL',
--'P1310-ROCKSTAR',
--'P1310-SCHW600',
--'P1310-SMITH175',
--'P1311-BREAK750',
--'P1311-CADMED',
--'P1311-CLASSBIGM',
--'P1311-COKE1.25',
--'P1311-COKE600',
--'P1311-CRIDGE',
--'P1311-DJUICE',
--'P1311-FREDDO',
--'P1311-FRNK1.5',
--'P1311-KIRK1.25',
--'P1311-MOTHER500',
--'P1311-NESTEA',
--'P1311-PADE',
--'P1311-RBULL500',
--'P1311-RSTAR',
--'P1311-SLINECOST',
--'P1311-STARLINE',
--'P1311-SWICHWTR',
--'P1311-V250',
--'P1312-COKE2LT',
--'P1312-COKE500',
--'P1312-CRDIDGE',
--'P1312-DARE',
--'P1312-FNTANGUS',
--'P1312-FRANK',
--'P1312-GATOR',
--'P1312-ICEBREAK',
--'P1312-ICEDTEA',
--'P1312-MARS2PK',
--'P1312-MAXIMUS',
--'P1312-MOTHER500',
--'P1312-NESMED',
--'P1312-RBULL473',
--'P1312-RROCK',
--'P1312-SCHW1.25',
--'P1312-SCHW600',
--'P1312-UPNGO-ENG',
--'P1312-UPNGO-LQD',
--'P1312-V250',
--'PEDS38',
--'PIZZADEAL',
--'RAHCADMED',
--'RICK002',
--'SCHW2F6',
--'SCHW2F6_',
--'SPARL19-P1',
--'SPARS02-X1',
--'SPARS02-X2',
--'SPARS02-X3',
--'SPARS04-X1',
--'SPARS04-X2',
--'SPARS06-X1',
--'SPARS08-X1',
--'SPARS08-X2',
--'SPARS08-X3',
--'SPARS09-X1',
--'SPARS09-X2',
--'SPARS09-X3',
--'SPARS09-X4',
--'SPARS09-X5',
--'SPARS12-X1',
--'SPARS12-X2',
--'SPARS12-X3',
--'SPARS14-X1',
--'SPARS14-X2',
--'SPARS14-X3',
--'SPARS14-X4',
--'SPARS15-X1',
--'SPARS15-X2',
--'SPARS15-X3',
--'SPARS16-X1',
--'SPARS16-X2',
--'SPARS16-X3',
--'SPARS16-X4',
--'SPARS16-X5',
--'SPARS18-X1',
--'SPARS19-X1',
--'SPARS20-X1',
--'SPARS21-X1',
--'SPARS22-X1',
--'SPARS24-X1',
--'SPARS24-X2',
--'SPARS24-X3',
--'SPARS26-X1',
--'SPARS26-X2',
--'SPARS28-X1',
--'SPARS30-X1',
--'SPARS32-783-X1',
--'SPARS32-X1',
--'SPARS34-X1',
--'SPARS34-X2',
--'SPARS36-X1',
--'SPARS38-X1',
--'SPARS39-X1',
--'SPARS39-X2',
--'SPARS46-X1',
--'SPORTICEDEAL',
--'TEST-QTY',
--'TEST-QTY-MM',
--'TIMTAM2FOR',
--'TIMTAM731',
--'TOASTIE',
--'TROLLILOLLY',
--'TRUBLU3FOR5')