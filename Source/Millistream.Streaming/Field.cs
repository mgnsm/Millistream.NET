﻿namespace Millistream.Streaming
{
    /// <summary>
    /// Fields / Tags
    /// </summary>
    public enum Field : uint
    {
        MDF_F_LANGUAGE = 0,
        MDF_F_HEADLINE = 1,
        MDF_F_TEXTBODY = 2,
        MDF_F_DATE = 3,
        MDF_F_TIME = 4,
        MDF_F_BIDPRICE = 5,
        MDF_F_ASKPRICE = 6,
        MDF_F_LASTPRICE = 7,
        MDF_F_DAYHIGHPRICE = 8,
        MDF_F_DAYLOWPRICE = 9,
        MDF_F_QUANTITY = 10,
        MDF_F_TURNOVER = 11,
        MDF_F_TRADEPRICE = 12,
        MDF_F_TRADEQUANTITY = 13,
        MDF_F_TRADEREFERENCE = 14,
        MDF_F_TRADECODE = 15,
        MDF_F_ORDERLEVEL = 16,
        MDF_F_NUMBIDORDERS = 17,
        MDF_F_NUMASKORDERS = 18,
        MDF_F_BIDQUANTITY = 19,
        MDF_F_ASKQUANTITY = 20,
        MDF_F_SYMBOL = 21,
        MDF_F_NAME = 22,
        MDF_F_ISIN = 23,
        MDF_F_BOARDLOT = 24,
        MDF_F_INSTRUMENTTYPE = 25,
        MDF_F_INSTRUMENTSUBTYPE = 26,
        MDF_F_DERIVATIVEINDICATOR = 27,
        MDF_F_EXERCISETYPE = 28,
        MDF_F_ISSUECURRENCY = 29,
        MDF_F_TRADECURRENCY = 30,
        MDF_F_BASECURRENCY = 31,
        MDF_F_QUOTECURRENCY = 32,
        MDF_F_ISSUEDATE = 33,
        MDF_F_STRIKEDATE = 34,
        MDF_F_STRIKEPRICE = 35,
        MDF_F_TRADETIME = 36,
        MDF_F_NUMTRADES = 37,
        MDF_F_EXECUTEDSIDE = 38,
        MDF_F_OPENPRICE = 39,
        MDF_F_CLOSEPRICE = 40,
        MDF_F_CLOSEBIDPRICE = 41,
        MDF_F_CLOSEASKPRICE = 42,
        MDF_F_CLOSEDAYHIGHPRICE = 43,
        MDF_F_CLOSEDAYLOWPRICE = 44,
        MDF_F_CLOSEQUANTITY = 45,
        MDF_F_CLOSETURNOVER = 46,
        MDF_F_CLOSENUMTRADES = 47,
        MDF_F_NEWSID = 48,
        MDF_F_REQUESTID = 49,
        MDF_F_REQUESTSTATUS = 50,
        MDF_F_REQUESTTYPE = 51,
        MDF_F_REQUESTCLASS = 52,
        MDF_F_INSREFLIST = 53,
        MDF_F_MARKETPLACE = 54,
        MDF_F_LIST = 55,
        MDF_F_INTERNALQUANTITY = 56,
        MDF_F_INTERNALTURNOVER = 57,
        MDF_F_CLOSEINTERNALQUANTITY = 58,
        MDF_F_CLOSEINTERNALTURNOVER = 59,
        MDF_F_TRADEBUYER = 60,
        MDF_F_TRADESELLER = 61,
        MDF_F_BIDCOUNTERPART = 62,
        MDF_F_ASKCOUNTERPART = 63,
        MDF_F_COMPANY = 64,
        MDF_F_FUNDPPMCODE = 65,
        MDF_F_UNDERLYINGID = 66,
        MDF_F_USERNAME = 67,
        MDF_F_PASSWORD = 68,
        MDF_F_EXTRACREDENTIAL = 69,
        MDF_F_LOGOFFREASON = 70,
        MDF_F_TRADETYPE = 71,
        MDF_F_TRADECANCELTIME = 72,
        MDF_F_NEWSBLOCKNUMBER = 73,
        MDF_F_BIDYIELD = 74,
        MDF_F_ASKYIELD = 75,
        MDF_F_LASTYIELD = 76,
        MDF_F_OPENYIELD = 77,
        MDF_F_DAYHIGHYIELD = 78,
        MDF_F_DAYLOWYIELD = 79,
        MDF_F_CLOSEBIDYIELD = 80,
        MDF_F_CLOSEASKYIELD = 81,
        MDF_F_CLOSEYIELD = 82,
        MDF_F_CLOSEDAYHIGHYIELD = 83,
        MDF_F_CLOSEDAYLOWYIELD = 84,
        MDF_F_NEWSCODINGCOMPANY = 85,
        MDF_F_NEWSCODINGTYPE = 86,
        MDF_F_NEWSCODINGSUBJECT = 87,
        MDF_F_NEWSCODINGCOUNTRY = 88,
        MDF_F_NEWSCODINGORIGINAL = 89,
        MDF_F_FUNDCOMPANY = 90,
        MDF_F_FUNDPMICODE = 91,
        MDF_F_COUNTRY = 92,
        MDF_F_NAV = 93,
        MDF_F_CLOSENAV = 94,
        MDF_F_TIS = 95,
        MDF_F_CLOSETIS = 96,
        MDF_F_SOURCE = 97,
        MDF_F_S1 = 98,
        MDF_F_CATYPE = 99,
        MDF_F_DIVIDEND = 100,
        /* MDF_F_DIVIDENDORIGINAL = 101 */
        MDF_F_CASUBTYPE = 102,
        MDF_F_ADJUSTMENTFACTOR = 103,
        MDF_F_NUMBEROFSHARES = 104,
        MDF_F_NUMBEROFSHARESDELTA = 105,
        MDF_F_NEWSHARES = 106,
        MDF_F_OLDSHARES = 107,
        MDF_F_SUBSCRIPTIONPRICE = 108,
        MDF_F_PERIOD = 109,
        MDF_F_NOMINALVALUE = 110,
        MDF_F_RECORDDATE = 111,
        MDF_F_PAYMENTDATE = 112,
        MDF_F_ANNOUNCEMENTDATE = 113,
        MDF_F_TID = 114,
        MDF_F_NEWSISLASTBLOCK = 115,
        MDF_F_SERVERNAME = 116,
        MDF_F_SERVERTIME = 117,
        MDF_F_SERVERDATE = 118,
        MDF_F_MIC = 119,
        MDF_F_UNCHANGEDPAID = 120,
        MDF_F_PLUSPAID = 121,
        MDF_F_MINUSPAID = 122,
        MDF_F_VWAP = 123,
        MDF_F_CLOSEVWAP = 124,
        MDF_F_SPECIALCONDITION = 125,
        MDF_F_TRADESTATE = 126,
        MDF_F_SALES = 127,
        MDF_F_EBIT = 128,
        MDF_F_PRETAXPROFIT = 129,
        MDF_F_NETPROFIT = 130,
        MDF_F_EPS = 131,
        MDF_F_DILUTEDEPS = 132,
        MDF_F_EBITDA = 133,
        MDF_F_EBITA = 134,
        /* MDF_F_ORDERINTAKE = 135,
        MDF_F_ORDERBACKLOG = 136,
        MDF_F_NETINTERESTINCOME = 137 */
        MDF_F_NETFININCOME = 138,
        /* MDF_F_NETFEEANDCOMINCOME 139,
        MDF_F_TOPERATINGEXPENSES = 140,
        MDF_F_TOPERATINGINCOME = 141,
        MDF_F_PROFITBEFOREWACL = 142,
        MDF_F_CREDITLOSS = 143,
        MDF_F_RENTALINCOME = 144,
        MDF_F_PROPERTYMGMTRESULT = 145,
        MDF_F_REALIZEDCHGPROP = 146,
        MDF_F_UNREALIZEDCHGPROP = 147 */
        MDF_F_CLOSEPRICE1D = 148,
        MDF_F_CLOSEPRICE1W = 149,
        MDF_F_CLOSEPRICE1M = 150,
        MDF_F_CLOSEPRICE3M = 151,
        MDF_F_CLOSEPRICE6M = 152,
        MDF_F_CLOSEPRICE9M = 153,
        MDF_F_CLOSEPRICE1Y = 154,
        MDF_F_CLOSEPRICE2Y = 155,
        MDF_F_CLOSEPRICE5Y = 156,
        MDF_F_CLOSEPRICE10Y = 157,
        MDF_F_CLOSEPRICEWTD = 158,
        MDF_F_CLOSEPRICEMTD = 159,
        MDF_F_CLOSEPRICEQTD = 160,
        MDF_F_CLOSEPRICEYTD = 161,
        MDF_F_CLOSEPRICEPYTD = 162,
        MDF_F_ATH = 163,
        MDF_F_ATL = 164,
        MDF_F_HIGHPRICE1Y = 165,
        MDF_F_LOWPRICE1Y = 166,
        MDF_F_NEWSCODINGISIN = 167,
        MDF_F_CHAIRMAN = 168,
        MDF_F_CEO = 169,
        MDF_F_WEBSITE = 170,
        MDF_F_ORGNUM = 171,
        MDF_F_DESCRIPTION = 172,
        MDF_F_EQUITYRATIO = 173,
        /* MDF_F_RETURNONEQUITY = 174 */
        MDF_F_DIVIDENDYIELD = 175,
        MDF_F_PER = 176,
        MDF_F_PSR = 177,
        MDF_F_S2 = 179,
        MDF_F_S3 = 180,
        MDF_F_S4 = 181,
        MDF_F_S5 = 182,
        MDF_F_ATHDATE = 183,
        MDF_F_ATLDATE = 184,
        MDF_F_HIGHPRICE1YDATE = 185,
        MDF_F_LOWPRICE1YDATE = 186,
        MDF_F_REDEMPTIONPRICE = 187,
        MDF_F_SECTOR = 188,
        MDF_F_OPERATINGCASHFLOW = 189,
        /* MDF_F_ADJUSTEDEQUITY = 190 */
        MDF_F_PRICETOCASHFLOW = 191,
        MDF_F_PRICETOADJUSTEDEQUITY = 192,
        MDF_F_HIGHPRICEYTD = 193,
        MDF_F_LOWPRICEYTD = 194,
        MDF_F_HIGHPRICEYTDDATE = 195,
        MDF_F_LOWPRICEYTDDATE = 196,
        MDF_F_COUNT = 197,
        MDF_F_GROSSPROFIT = 198,
        MDF_F_NETSALES = 199,
        MDF_F_ADJUSTEDEBITA = 200,
        MDF_F_TRADEYIELD = 201,
        MDF_F_VOTINGPOWERPRC = 202,
        MDF_F_CAPITALPRC = 203,
        MDF_F_GENDERCEO = 204,
        MDF_F_GENDERCHAIRMAN = 205,
        MDF_F_BIRTHYEARCEO = 206,
        MDF_F_BIRTHYEARCHAIRMAN = 207,
        MDF_F_ADDRESS = 208,
        MDF_F_POSTALCODE = 209,
        MDF_F_CITY = 210,
        MDF_F_TELEPHONE = 211,
        MDF_F_FAX = 212,
        MDF_F_EMAIL = 213,
        MDF_F_IMPORTANTEVENTS = 214,
        MDF_F_INTANGIBLEASSET = 215,
        MDF_F_GOODWILL = 216,
        MDF_F_FIXEDASSET = 217,
        MDF_F_FINANCIALASSET = 218,
        MDF_F_NONCURRENTASSET = 219,
        MDF_F_INVENTORY = 220,
        MDF_F_OTHERCURRENTASSET = 221,
        MDF_F_ACCOUNTSRECEIVABLE = 222,
        MDF_F_OTHERRECEIVABLES = 223,
        MDF_F_SHORTTERMINV = 224,
        MDF_F_CCE = 225,
        MDF_F_CURRENTASSETS = 226,
        MDF_F_TOTALASSETS = 227,
        MDF_F_SHEQUITY = 228,
        MDF_F_MINORITYINTEREST = 229,
        MDF_F_PROVISIONS = 230,
        MDF_F_LTLIABILITIES = 231,
        MDF_F_CURLIABILITIES = 232,
        MDF_F_TOTSHEQLIABILITIES = 233,
        MDF_F_NIBL = 234,
        /* MDF_F_TOTLIABILITIES = 235 */
        MDF_F_IBL = 236,
        MDF_F_CASHFLOWBWC = 237,
        MDF_F_CASHFLOWAWC = 238,
        MDF_F_CASHFLOWIA = 239,
        MDF_F_CASHFLOWFA = 240,
        MDF_F_CASHFLOWTOTAL = 241,
        MDF_F_NUMEMPLOYEES = 242,
        MDF_F_MCAP = 243,
        MDF_F_CONTRACTSIZE = 244,
        MDF_F_BASERATIO = 245,
        MDF_F_SOURCEID = 246,
        MDF_F_ISSUER = 247,
        MDF_F_GENIUMID = 248,
        MDF_F_CLOSEPRICE3Y = 249,
        MDF_F_CLOSEPRICELD = 250,
        MDF_F_FUNDYEARLYMGMTFEE = 251,
        MDF_F_FUNDPPMFEE = 252,
        MDF_F_FUNDPPMTYPE = 253,
        MDF_F_FUNDBENCHMARK = 254,
        MDF_F_FUNDLEVERAGE = 255,
        MDF_F_FUNDDIRECTION = 256,
        MDF_F_PROSPECTUS = 257,
        MDF_F_GEOFOCUSREGION = 258,
        MDF_F_GEOFOCUSCOUNTRY = 259,
        MDF_F_OPENINTEREST = 260,
        MDF_F_CLOSEYIELD1D = 261,
        MDF_F_CLOSEYIELD1W = 262,
        MDF_F_CLOSEYIELD1M = 263,
        MDF_F_CLOSEYIELD3M = 264,
        MDF_F_CLOSEYIELD6M = 265,
        MDF_F_CLOSEYIELD9M = 266,
        MDF_F_CLOSEYIELD1Y = 267,
        MDF_F_CLOSEYIELD2Y = 268,
        MDF_F_CLOSEYIELD3Y = 269,
        MDF_F_CLOSEYIELD5Y = 270,
        MDF_F_CLOSEYIELD10Y = 271,
        MDF_F_CLOSEYIELDWTD = 272,
        MDF_F_CLOSEYIELDMTD = 273,
        MDF_F_CLOSEYIELDQTD = 274,
        MDF_F_CLOSEYIELDYTD = 275,
        MDF_F_CLOSEYIELDPYTD = 276,
        MDF_F_CLOSEYIELDLD = 277,
        MDF_F_ATHYIELD = 278,
        MDF_F_ATLYIELD = 279,
        MDF_F_ATHYIELDDATE = 280,
        MDF_F_ATLYIELDDATE = 281,
        MDF_F_HIGHYIELD1Y = 282,
        MDF_F_LOWYIELD1Y = 283,
        MDF_F_HIGHYIELDYTD = 284,
        MDF_F_LOWYIELDYTD = 285,
        MDF_F_HIGHYIELDYTDDATE = 286,
        MDF_F_LOWYIELDYTDDATE = 287,
        MDF_F_HIGHYIELD1YDATE = 288,
        MDF_F_LOWYIELD1YDATE = 289,
        MDF_F_CUSIP = 290,
        MDF_F_WKN = 291,
        MDF_F_UCITS = 292,
        MDF_F_INCEPTIONDATE = 293,
        MDF_F_FUNDBENCHMARKINSREF = 294,
        MDF_F_INSTRUMENTCLASS = 295,
        MDF_F_INSTRUMENTSUBCLASS = 296,
        MDF_F_CONSTITUENTS = 297,
        MDF_F_COUPONRATE = 298,
        MDF_F_COUPONDATE = 299,
        MDF_F_BARRIERPRICE = 300,
        MDF_F_STANDARDDEVIATION3Y = 301,
        MDF_F_ANNUALIZEDRETURN3Y = 302,
        MDF_F_SHARPERATIO3Y = 303,
        MDF_F_MORNINGSTARRATING = 304,
        MDF_F_SALESFEE = 305,
        MDF_F_PURCHASEFEE = 306,
        MDF_F_MINSTARTAMOUNT = 307,
        MDF_F_MINSUBSCRIPTIONAMOUNT = 308,
        MDF_F_PERFORMANCEFEE = 309,
        MDF_F_MINADDITIONALAMOUNT = 310,
        MDF_F_ANNUALIZEDRETURN5Y = 311,
        MDF_F_ANNUALIZEDRETURN10Y = 312,
        MDF_F_CEOADMISSIONDATE = 313,
        MDF_F_CHAIRMANADMISSIONDATE = 314,
        MDF_F_TRADEDTHROUGHDATE = 315,
        MDF_F_TOTALFEE = 316,
        MDF_F_DIVIDENDTYPE = 317,
        MDF_F_DIVIDENDFREQUENCY = 318,
        MDF_F_INSTRUMENTSUBSUBCLASS = 319,
        MDF_F_PRIMARYMARKETPLACE = 320,
        MDF_F_FISCALPERIOD = 321,
        MDF_F_SHORTDESCRIPTION = 322,
        MDF_F_FUNDRISK = 323,
        MDF_F_EUSIPA = 324,
        MDF_F_NEWSRANK = 325,
        MDF_F_AVERAGE = 326,
        MDF_F_MIN = 327,
        MDF_F_MAX = 328,
        MDF_F_FIELDNAME = 329,
        MDF_F_FIELDASPECT = 330,
        MDF_F_FIELDTYPE = 331,
        MDF_F_FUNDCOMPANY2 = 332,
        MDF_F_FIELDUNIT = 333,
        MDF_F_CLOSEPRICE2W = 334,
        MDF_F_CLOSEYIELD2W = 335,
        MDF_F_CONVERTFROMDATE = 336,
        MDF_F_CONVERTTODATE = 337,
        MDF_F_CONVERSIONPRICE = 338,
        MDF_F_DURATION = 339,
        MDF_F_SETTLEMENTTYPE = 340,
        MDF_F_VOTINGPOWER = 341,
        MDF_F_CAP = 342,
        MDF_F_IMBALANCE = 343,
        MDF_F_IMBALANCEDIRECTION = 344,
        MDF_F_CROSSTYPE = 345,
        MDF_F_TICKTABLE = 346,
        MDF_F_TICKSIZES = 347,
        MDF_F_PRICETYPE = 348,
        MDF_F_ASIANTAILSTART = 349,
        MDF_F_ASIANTAILEND = 350,
        MDF_F_LOGOTYPE = 351,
        MDF_F_ISSUERNAME = 352,
        MDF_F_CONTRACTVALUE = 353,
        MDF_F_CLOSEBIDPRICE1D = 354,
        MDF_F_CLOSEBIDYIELD1D = 355,
        MDF_F_CLOSEBIDPRICE1W = 356,
        MDF_F_CLOSEBIDYIELD1W = 357,
        MDF_F_FINANCIALINCOME = 358,
        MDF_F_FINANCIALCOST = 359,
        MDF_F_FINANCINGLEVEL = 360,
        MDF_F_PARTICIPATIONRATE = 361,
        MDF_F_ISSUEPRICE = 362,
        MDF_F_FIINSTITUTENUMBER = 363,
        MDF_F_DELETERECORD = 364,
        MDF_F_KIID = 365,
        MDF_F_CFI = 366,
        MDF_F_OFFBOOKQUANTITY = 367,
        MDF_F_OFFBOOKTURNOVER = 368,
        MDF_F_DARKQUANTITY = 369,
        MDF_F_DARKTURNOVER = 370,
        MDF_F_CLOSEOFFBOOKQUANTITY = 371,
        MDF_F_CLOSEOFFBOOKTURNOVER = 372,
        MDF_F_CLOSEDARKQUANTITY = 373,
        MDF_F_CLOSEDARKTURNOVER = 374,
        MDF_F_BROKERS = 375,
        MDF_F_INTERESTINCOME = 376,
        MDF_F_OTHERFINANCIALINCOME = 377,
        MDF_F_INTERESTEXPENSE = 378,
        MDF_F_OTHERFINANCIALEXPENSE = 379,
        MDF_F_MINORITYINTERESTRES = 380,
        MDF_F_ACCOUNTSPAYABLE = 381,
        MDF_F_EVENTLINK = 382,
        MDF_F_EVENTLINKLANGUAGES = 383,
        MDF_F_MAXLEVEL = 384,
        MDF_F_SETTLEMENTPRICE = 385,
        MDF_F_ANNUALIZEDRETURN1Y = 386,
        MDF_F_ANNUALIZEDRETURN2Y = 387,
        MDF_F_ANNUALIZEDRETURN4Y = 388,
        MDF_F_S6 = 389,
        MDF_F_S7 = 390,
        MDF_F_S8 = 391,
        MDF_F_S9 = 392,
        MDF_F_S10 = 393,
        MDF_F_N1 = 394,
        MDF_F_N2 = 395,
        MDF_F_N3 = 396,
        MDF_F_N4 = 397,
        MDF_F_N5 = 398,
        MDF_F_I1 = 399,
        MDF_F_I2 = 400,
        MDF_F_I3 = 401,
        MDF_F_I4 = 402,
        MDF_F_I5 = 403,
        MDF_F_D1 = 404,
        MDF_F_D2 = 405,
        MDF_F_D3 = 406,
        MDF_F_CITYPE = 407,
        MDF_F_CISUBTYPE = 408,
        MDF_F_SEQUENCE = 409,
        MDF_F_OUTSTANDINGAMOUNT = 410,
        MDF_F_INTERESTRATE = 411,
        MDF_F_MARKETMAKER = 412,
        MDF_F_SUSTAINDESCRIPTION = 413,
        MDF_F_SUSTAININVESTING1 = 414,
        MDF_F_SUSTAININVESTING2 = 415,
        MDF_F_SUSTAINDIVESTING1 = 416,
        MDF_F_SUSTAINDIVESTING2 = 417,
        MDF_F_SUSTAININFLUENCE = 418,
        MDF_F_SUSTAINMONITORING1 = 419,
        MDF_F_SUSTAINMONITORING2 = 420,
        MDF_F_SUSTAINCOMMENT = 421,
        MDF_F_NUMBEROFPREFSHARES = 422,
        MDF_F_MARKETOPEN = 423,
        MDF_F_MARKETCLOSE = 424,
        MDF_F_MARKETEARLYCLOSE = 425,
        MDF_F_LEGALSTRUCTURE = 426,
        MDF_F_ONGOINGCHARGE = 427,
        MDF_F_PRICINGFREQUENCY = 428,
        MDF_F_MARKETOPENDAY = 429,
        MDF_F_CLOSETRADEPRICE = 430,
        MDF_F_CLOSEPRICETYPE = 431,
        MDF_F_NETDIVIDEND = 432,
        MDF_F_PRODUCTCODE = 433,
        MDF_F_QUOTINGTYPE = 434,
        MDF_F_TRADEAGREEMENTTIME = 435,
        MDF_F_TRADEAGREEMENTDATE = 436,
        MDF_F_LATESTYEARENDREPORT = 437,
        MDF_F_VOLUMEDIMENSION = 438,
        MDF_F_NEWSCODINGREGULATORY = 439,
        MDF_F_NORMANAMOUNT = 440,
        MDF_F_CSR = 441,
        MDF_F_S11 = 442,
        MDF_F_CIK = 443,
        MDF_F_PRIIP00010 = 444,
        MDF_F_PRIIP00020 = 445,
        MDF_F_PRIIP00030 = 446,
        MDF_F_PRIIP00040 = 447,
        MDF_F_PRIIP00050 = 448,
        MDF_F_PRIIP00060 = 449,
        MDF_F_PRIIPKID = 450,
        MDF_F_PRIIP00080 = 451,
        MDF_F_PRIIP00090 = 452,
        MDF_F_PRIIP00100 = 453,
        MDF_F_PRIIP01010 = 454,
        MDF_F_PRIIP01020 = 455,
        MDF_F_PRIIP01030 = 456,
        MDF_F_PRIIP01040 = 457,
        MDF_F_PRIIP01050 = 458,
        MDF_F_PRIIP01060 = 459,
        MDF_F_PRIIP01070 = 460,
        MDF_F_PRIIP01080 = 461,
        MDF_F_PRIIP01090 = 462,
        MDF_F_PRIIP01100 = 463,
        MDF_F_PRIIP01110 = 464,
        MDF_F_PRIIP01120 = 465,
        /* MDF_F_PRIIP01130 = 466 */
        MDF_F_PRIIP01140 = 467,
        MDF_F_PRIIP02010 = 468,
        MDF_F_PRIIP02020 = 469,
        MDF_F_PRIIP02030 = 470,
        MDF_F_PRIIP02040 = 471,
        MDF_F_PRIIP02050 = 472,
        MDF_F_PRIIP02060 = 473,
        MDF_F_PRIIP02070 = 474,
        MDF_F_PRIIP02080 = 475,
        MDF_F_PRIIP02090 = 476,
        MDF_F_PRIIP02100 = 477,
        MDF_F_PRIIP02110 = 478,
        MDF_F_PRIIP02120 = 479,
        MDF_F_PRIIP02130 = 480,
        MDF_F_PRIIP02140 = 481,
        MDF_F_PRIIP02150 = 482,
        MDF_F_PRIIP02160 = 483,
        MDF_F_PRIIP02170 = 484,
        MDF_F_PRIIP02180 = 485,
        MDF_F_PRIIP03010 = 486,
        MDF_F_PRIIP03015 = 487,
        MDF_F_PRIIP03020 = 488,
        MDF_F_PRIIP03030 = 489,
        MDF_F_PRIIP03040 = 490,
        MDF_F_PRIIP03050 = 491,
        MDF_F_PRIIP03060 = 492,
        MDF_F_PRIIP03070 = 493,
        MDF_F_PRIIP03080 = 494,
        MDF_F_PRIIP03090 = 495,
        MDF_F_PRIIP03095 = 496,
        MDF_F_PRIIP03100 = 497,
        MDF_F_PRIIP03105 = 498,
        MDF_F_SUBMARKET = 499,
        MDF_F_PRIIP04020 = 500,
        MDF_F_PRIIP04030 = 501,
        MDF_F_PRIIP04040 = 502,
        MDF_F_PRIIP04050 = 503,
        MDF_F_PRIIP04060 = 504,
        MDF_F_PRIIP04070 = 505,
        MDF_F_PRIIP04080 = 506,
        MDF_F_PRIIP04081 = 507,
        MDF_F_PRIIP04082 = 508,
        MDF_F_PRIIP04083 = 509,
        MDF_F_PRIIP04084 = 510,
        MDF_F_PRIIP04085 = 511,
        MDF_F_PRIIP04090 = 512,
        MDF_F_PRIIP04100 = 513,
        MDF_F_PRIIP04110 = 514,
        MDF_F_PRIIP05010 = 515,
        MDF_F_PRIIP05020 = 516,
        MDF_F_PRIIP05030 = 517,
        MDF_F_PRIIP05040 = 518,
        MDF_F_PRIIP05050 = 519,
        MDF_F_PRIIP05060 = 520,
        MDF_F_PRIIP05065 = 521,
        MDF_F_PRIIP05070 = 522,
        MDF_F_PRIIP05080 = 523,
        MDF_F_MMT = 524,
        MDF_F_FISN = 525,
        MDF_F_LEI = 526,
        MDF_F_SECTORMEMBERS = 527,
        MDF_F_PRIIP06010 = 528,
        MDF_F_PRIIP06020 = 529,
        MDF_F_PRIIP06030 = 530,
        MDF_F_PRIIP06040 = 531,
        MDF_F_PRIIP06050 = 532,
        MDF_F_PRIIP06060 = 533,
        MDF_F_PRIIP06070 = 534,
        MDF_F_PRIIP06080 = 535,
        MDF_F_PRIIP07010 = 536,
        MDF_F_PRIIP07020 = 537,
        MDF_F_PRIIP07030 = 538,
        MDF_F_PRIIP07040 = 539,
        MDF_F_PRIIP07050 = 540,
        MDF_F_PRIIP07060 = 541,
        MDF_F_PRIIP07070 = 542,
        MDF_F_PRIIP07080 = 543,
        MDF_F_PRIIP07090 = 544,
        MDF_F_PRIIP07100 = 545,
        MDF_F_PRIIP07110 = 546,
        MDF_F_PRIIP07120 = 547,
        MDF_F_MIFID00010 = 548,
        MDF_F_MIFID00020 = 549,
        MDF_F_MIFID00030 = 550,
        MDF_F_MIFID00040 = 551,
        MDF_F_NEWSIDREPLACE = 552,
        MDF_F_MIFID00060 = 553,
        MDF_F_MIFID00070 = 554,
        MDF_F_MIFID00080 = 555,
        MDF_F_MIFID00090 = 556,
        MDF_F_MIFID00100 = 557,
        MDF_F_MIFID01010 = 558,
        MDF_F_MIFID01020 = 559,
        MDF_F_MIFID01030 = 560,
        MDF_F_MIFID02010 = 561,
        MDF_F_MIFID02020 = 562,
        MDF_F_MIFID02030 = 563,
        MDF_F_MIFID02040 = 564,
        MDF_F_MIFID03010 = 565,
        MDF_F_MIFID03020 = 566,
        MDF_F_MIFID03030 = 567,
        MDF_F_MIFID03040 = 568,
        MDF_F_MIFID03050 = 569,
        MDF_F_MIFID04010 = 570,
        MDF_F_MIFID04020 = 571,
        MDF_F_MIFID04030 = 572,
        MDF_F_MIFID04040 = 573,
        MDF_F_MIFID04050 = 574,
        MDF_F_MIFID05010 = 575,
        MDF_F_MIFID05020 = 576,
        MDF_F_MIFID05030 = 577,
        MDF_F_MIFID05040 = 578,
        MDF_F_MIFID05050 = 579,
        MDF_F_MIFID05060 = 580,
        MDF_F_MIFID05070 = 581,
        MDF_F_MIFID05080 = 582,
        MDF_F_MIFID05080N = 583,
        MDF_F_ENTITLEMENTPACKAGE = 584,
        MDF_F_MIFID05100 = 585,
        MDF_F_MIFID05110 = 586,
        MDF_F_MIFID06010 = 587,
        MDF_F_MIFID06020 = 588,
        MDF_F_MIFID06030 = 589,
        MDF_F_MIFID06040 = 590,
        MDF_F_MIFID07010 = 591,
        MDF_F_MIFID07020 = 592,
        MDF_F_MIFID07030 = 593,
        MDF_F_MIFID07040 = 594,
        MDF_F_MIFID07050 = 595,
        MDF_F_MIFID07060 = 596,
        MDF_F_MIFID07070 = 597,
        MDF_F_MIFID07080 = 598,
        MDF_F_MIFID07090 = 599,
        MDF_F_MIFID07100 = 600,
        MDF_F_MIFID07110 = 601,
        MDF_F_MIFID07120 = 602,
        MDF_F_MIFID07130 = 603,
        MDF_F_MIFID07140 = 604,
        MDF_F_MIFID08010 = 605,
        MDF_F_MIFID08020 = 606,
        MDF_F_MIFID08030 = 607,
        MDF_F_MIFID08040 = 608,
        MDF_F_MIFID08050 = 609,
        MDF_F_MIFID08060 = 610,
        MDF_F_MIFID08070 = 611,
        MDF_F_MIFID08080 = 612,
        MDF_F_MIFID08090 = 613,
        MDF_F_MIFID08100 = 614,
        MDF_F_OPERATINGMIC = 615,
        MDF_F_CVR = 616,
        MDF_F_KENNITALA = 617,
        MDF_F_YTUNNUS = 618,
        MDF_F_ORGNUMNO = 619,
        MDF_F_NEWSCODINGTAGS = 620,
        MDF_F_NEWSCODINGIMPACT = 621,
        MDF_F_ACTIVESHARE = 622,
        MDF_F_TRACKINGERROR = 623,
        MDF_F_HIGHPRICE3Y = 624,
        MDF_F_LOWPRICE3Y = 625,
        MDF_F_HIGHPRICE5Y = 626,
        MDF_F_LOWPRICE5Y = 627,
        MDF_F_HIGHPRICE10Y = 628,
        MDF_F_LOWPRICE10Y = 629,
        MDF_F_DPS = 630,
        MDF_F_CFPS = 631,
        MDF_F_CFPSTTM = 632,
        MDF_F_CFPSLAST = 633,
        MDF_F_EPSTTM = 634,
        MDF_F_EPSLAST = 635,
        MDF_F_SPS = 636,
        MDF_F_SPSTTM = 637,
        MDF_F_SPSLAST = 638,
        MDF_F_BVPS = 639,
        MDF_F_BVPSLAST = 640,
        MDF_F_ROE = 641,
        MDF_F_ROETTM = 642,
        MDF_F_ROELAST = 643,
        MDF_F_ROA = 644,
        MDF_F_ROATTM = 645,
        MDF_F_ROALAST = 646,
        MDF_F_GM = 647,
        MDF_F_GMTTM = 648,
        MDF_F_GMLAST = 649,
        MDF_F_OM = 650,
        MDF_F_OMTTM = 651,
        MDF_F_OMLAST = 652,
        MDF_F_PM = 653,
        MDF_F_PMTTM = 654,
        MDF_F_PMLAST = 655,
        MDF_F_MIFID00001 = 656,
        MDF_F_MIFID00005 = 657,
        MDF_F_MIFID00006 = 658,
        MDF_F_MIFID00007 = 659,
        MDF_F_MIFID00008 = 660,
        MDF_F_MIFID00045 = 661,
        MDF_F_MIFID00047 = 662,
        MDF_F_MIFID00068 = 663,
        MDF_F_MIFID00069 = 664,
        MDF_F_MIFID00073 = 665,
        MDF_F_MIFID00074 = 666,
        MDF_F_MIFID00075 = 667,
        MDF_F_MIFID00085 = 668,
        MDF_F_MIFID00095 = 669,
        MDF_F_MIFID00110 = 670,
        MDF_F_MIFID00120 = 671,
        MDF_F_MIFID00130 = 672,
        MDF_F_MIFID01000 = 673,
        MDF_F_MIFID05105 = 674,
        MDF_F_MIFID05115 = 675,
        MDF_F_MIFID07025 = 676,
        MDF_F_MIFID07101 = 677,
        MDF_F_MIFID07105 = 678,
        MDF_F_MIFID07150 = 679,
        MDF_F_MIFID07155 = 680,
        MDF_F_MIFID07160 = 681,
        MDF_F_MIFID08015 = 682,
        MDF_F_MIFID08025 = 683,
        MDF_F_MIFID08045 = 684,
        MDF_F_MIFID08046 = 685,
        MDF_F_MIFID08085 = 686,
        MDF_F_MIFID08110 = 687,
        MDF_F_MIFID08120 = 688,
        MDF_F_FIGI = 689,
        MDF_F_FIGICOMPOSITE = 690,
        MDF_F_FIGISHARECLASS = 691,
        MDF_F_MMO = 692,
        MDF_F_FIGISECURITYTYPE = 693,
        MDF_F_COUPONFREQUENCY = 694,
        MDF_F_ORDERID = 695,
        MDF_F_ORDERIDSOURCE = 696,
        MDF_F_ORDERPRIORITY = 697,
        MDF_F_IV = 698,
        MDF_F_DELTA = 699,
        MDF_F_GAMMA = 700,
        MDF_F_RHO = 701,
        MDF_F_THETA = 702,
        MDF_F_VEGA = 703,
        MDF_F_ORDERSIDE = 704,
        MDF_F_ORDERPARTICIPANT = 705,
        MDF_F_ORDERPRICE = 706,
        MDF_F_ORDERQUANTITY = 707,
        MDF_F_IVBID = 708,
        MDF_F_IVASK = 709,
        MDF_F_DAYCOUNTCONVENTION = 710,
        MDF_F_EXPIRATIONTYPE = 711,
        MDF_F_TENOR = 712,
        MDF_F_S12 = 713,
        MDF_F_S13 = 714,
        MDF_F_S14 = 715,
        MDF_F_S15 = 716,
        MDF_F_N6 = 717,
        MDF_F_N7 = 718,
        MDF_F_N8 = 719,
        MDF_F_N9 = 720,
        MDF_F_N10 = 721,
        MDF_F_I6 = 722,
        MDF_F_I7 = 723,
        MDF_F_I8 = 724,
        MDF_F_I9 = 725,
        MDF_F_I10 = 726,
        MDF_F_INSREF1 = 727,
        MDF_F_INSREF2 = 728,
        MDF_F_INSREF3 = 729,
        MDF_F_INSREF4 = 730,
        MDF_F_INSREF5 = 731
    }
}