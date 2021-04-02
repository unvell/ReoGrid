/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if FORMULA

namespace unvell.ReoGrid.Formula
{
	/// <summary>
	/// Constants of all built-in function name.
	/// </summary>
	static class BuiltinFunctionNames
	{


		public const string SUM_EN = "SUM";
		public const string SUM_RU = "СУММ";

		public const string AVERAGE_EN = "AVERAGE";
		public const string AVERAGE_RU = "СРЗНАЧ";
		public const string COUNT_EN = "COUNT";
		public const string COUNT_RU = "СЧЁТ";
		public const string COUNTA_EN = "COUNTA";
		public const string COUNTA_RU = "СЧЁТЗ";
		public const string MIN_EN = "MIN";
		public const string MIN_RU = "МИН";
		public const string MAX_EN = "MAX";
		public const string MAX_RU = "МАКС";

		public const string IF_EN = "IF";
		public const string IF_RU = "ЕСЛИ";
		public const string AND_EN = "AND";
		public const string AND_RU = "И";
		public const string OR_EN = "OR";
		public const string OR_RU = "ИЛИ";
		public const string NOT_EN = "NOT";
		public const string NOT_RU = "НЕ";

		public const string ROW_EN = "ROW";
		public const string ROW_RU = "СТРОКА";
		public const string COLUMN_EN = "COLUMN";
		public const string COLUMN_RU = "СТОЛБЕЦ";
		public const string ADDRESS_EN = "ADDRESS";
		public const string ADDRESS_RU = "АДРЕС";
		public const string INDIRECT_EN = "INDIRECT";
		public const string INDIRECT_RU = "ДВССЫЛ";

		public const string ABS_EN = "ABS";
		public const string ABS_RU = "ABS";
		public const string ROUND_EN = "ROUND";
		public const string ROUND_RU = "ОКРУГЛ";
		public const string CEILING_EN = "CEILING";
		public const string CEILING_RU = "ОКРВВЕРХ";
		public const string FLOOR_EN = "FLOOR";
		public const string FLOOR_RU = "ОКРВНИЗ";
		public const string SIN_EN = "SIN";
		public const string SIN_RU = "SIN";
		public const string COS_EN = "COS";
		public const string COS_RU = "COS";
		public const string TAN_EN = "TAN";
		public const string TAN_RU = "TAN";
		public const string ASIN_EN = "ASIN";
		public const string ASIN_RU = "ASIN";
		public const string ACOS_EN = "ACOS";
		public const string ACOS_RU = "ACOS";
		public const string ATAN_EN = "ATAN";
		public const string ATAN_RU = "ATAN";
		public const string ATAN2_EN = "ATAN2";
		public const string ATAN2_RU = "ATAN2";
		public const string POWER_EN = "POWER";
		public const string POWER_RU = "СТЕПЕНЬ";
		public const string EXP_EN = "EXP";
		public const string EXP_RU = "EXP";
		public const string LOG_EN = "LOG";
		public const string LOG_RU = "LOG";
		public const string LOG10_EN = "LOG10";
		public const string LOG10_RU = "LOG10";
		public const string MOD_EN = "MOD";
		public const string MOD_RU = "ОСТАТ";

		public const string NOW_EN = "NOW";
		public const string NOW_RU = "ТДАТА";
		public const string TODAY_EN = "TODAY";
		public const string TODAY_RU = "СЕГОДНЯ";
		public const string TIME_EN = "TIME";
		public const string TIME_RU = "ВРЕМЯ";
		public const string YEAR_EN = "YEAR";
		public const string YEAR_RU = "ГОД";
		public const string MONTH_EN = "MONTH";
		public const string MONTH_RU = "МЕСЯЦ";
		public const string DAY_EN = "DAY";
		public const string DAY_RU = "ДЕНЬ";
		public const string HOUR_EN = "HOUR";
		public const string HOUR_RU = "ЧАС";
		public const string MINUTE_EN = "MINUTE";
		public const string MINUTE_RU = "МИНУТЫ";
		public const string SECOND_EN = "SECOND";
		public const string SECOND_RU = "СЕКУНДЫ";
		public const string MILLISECOND_EN = "MILLISECOND";
		public const string MILLISECOND_RU = "МИЛЛИСЕКУНДЫ";
		public const string DAYS_EN = "DAYS";
		public const string DAYS_RU = "ДНЕЙ";

		public const string LEFT_EN = "LEFT";
		public const string LEFT_RU = "ЛЕВСИМВ";
		public const string RIGHT_EN = "RIGHT";
		public const string RIGHT_RU = "ПРАВСИМВ";
		public const string MID_EN = "MID";
		public const string MID_RU = "ПСТР";
		public const string UPPER_EN = "UPPER";
		public const string UPPER_RU = "ПРОПИСН";
		public const string LOWER_EN = "LOWER";
		public const string LOWER_RU = "СТРОЧН";
		public const string LEN_EN = "LEN";
		public const string LEN_RU = "ДЛСТР";
		public const string FIND_EN = "FIND";
		public const string FIND_RU = "НАЙТИ";
		public const string TRIM_EN = "TRIM";
		public const string TRIM_RU = "СЖПРОБЕЛЫ";

		public const string ISERROR_EN = "ISERROR";
		public const string ISERROR_RU = "ЕСЛИОШИБКА";
		public const string ISNUMBER_EN = "ISNUMBER";
		public const string ISNUMBER_RU = "ЕЧИСЛО";

		public const string SUMIF_EN = "SUMIF";
		public const string SUMIF_RU = "СУММЕСЛИ";
		public const string AVERAGEIF_EN = "AVERAGEIF";
		public const string AVERAGEIF_RU = "СРЗНАЧЕСЛИ";
		public const string COUNTIF_EN = "COUNTIF";
		public const string COUNTIF_RU = "СЧЁТЕСЛИ";
		public const string VLOOKUP_EN = "VLOOKUP";
		public const string VLOOKUP_RU = "ВПР";


	}
}

#endif // FORMULA