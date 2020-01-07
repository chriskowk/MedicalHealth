using Core.MyOle2.Metadata;
using Core.MyOle2;
using Core.MyXls.ByteUtil;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Io = System.IO;

namespace Core.MyXls
{
    /// <summary>
    /// Contains constant values pertaining to the BIFF8 format for use in MyXLS.
    /// </summary>
    public static class BIFF8
    {
        /// <summary>
        /// The name of the Workbook stream in an OLE2 Document.
        /// </summary>
        public static readonly byte[] NameWorkbook = new byte[] {
            0x57, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6B, 0x00, 0x62, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6B, 0x00,
            0x00, 0x00};

        /// <summary>
        /// The name of the SummaryInformation stream in an OLE2 Document.
        /// </summary>
        public static readonly byte[] NameSummaryInformation = new byte[] {
            0x05, 0x00, 0x53, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00,
            0x49, 0x00, 0x6E, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x74, 0x00,
            0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00};

        /// <summary>
        /// The name of the DocumentSummaryInformation stream in an OLE2 Document.
        /// </summary>
        public static readonly byte[] NameDocumentSummaryInformation = new byte[] {
            0x05, 0x00, 0x44, 0x00, 0x6F, 0x00, 0x63, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x6E, 0x00,
            0x74, 0x00, 0x53, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00,
            0x49, 0x00, 0x6E, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x74, 0x00,
            0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00};

        /// <summary>
        /// The maximum rows a BIFF8 document may contain.
        /// </summary>
        public const ushort MaxRows = ushort.MaxValue;

        /// <summary>
        /// The maximum columns a BIFF8 document may contain.
        /// </summary>
        public const ushort MaxCols = byte.MaxValue;

        /// <summary>
        /// The maximum number of bytes in a BIFF8 record (minus 4 bytes for 
        /// the Record ID and the data size, leaves 8224 bytes for data).
        /// </summary>
        public const ushort MaxBytesPerRecord = 8228;

        /// <summary>
        /// The maximum number of bytes available for data in a BIFF8 record
        /// (plus 4 bytes for the Record ID and the data size, gives 8228 total
        /// bytes).
        /// </summary>
        public const ushort MaxDataBytesPerRecord = 8224;

        /// <summary>
        /// The Maximum number of characters that can be written to or read
        /// from a Cell in Excel.  I'm guessing it is short.MaxValue instead 
        /// of ushort.MaxValue to allow for double-byte chars (Unicode).
        /// </summary>
        public const ushort MaxCharactersPerCell = (ushort)short.MaxValue;
    }
}
namespace Core.MyXls
{
    internal struct CachedBlockRow
    {
        internal ushort RowBlockIndex;
        internal ushort BlockRowIndex;
        internal Row Row;
        internal static CachedBlockRow Empty = new CachedBlockRow(0, 0, null);

        internal CachedBlockRow(ushort rowBlockIndex, ushort blockRowIndex, Row row)
        {
            RowBlockIndex = rowBlockIndex;
            BlockRowIndex = blockRowIndex;
            Row = row;
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// A single cell on or to be added to a worksheet.
    /// </summary>
    public class Cell : IXFTarget
    {
        private Worksheet _worksheet;

        private object _value;
        private CellTypes _type = CellTypes.Null;
        private ushort _row = 0;
        private ushort _column = 0;
        private CellCoordinate _coordinate;
        private int _xfIdx = -1;

        internal Cell(Worksheet worksheet)
        {
            _worksheet = worksheet;
        }

        //NOTE: Could do this another way (used by Worksheet.AddCell)
        /// <summary>
        /// Gets or sets the CellCoordinate of this Cell object.
        /// </summary>
        public CellCoordinate Coordinate
        {
            get { return _coordinate; }
            set
            {
                _coordinate = value;
                _row = value.Row;
                _column = value.Column;
            }
        }

        /// <summary>
        /// Gets the Row value of this Cell (1-based).
        /// </summary>
        public ushort Row
        {
            get { return _row; }
        }

        /// <summary>
        /// Gets the Column value of this Cell (1-based).
        /// </summary>
        public ushort Column
        {
            get { return _column; }
        }

        /// <summary>
        /// Gets the type of this Cell.
        /// </summary>
        public CellTypes Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets or sets the Value in this Cell.
        /// </summary>
        public object Value
        {
            get
            {
                if (_type == CellTypes.Text && !(_value is string))
                    return _worksheet.Document.Workbook.SharedStringTable.GetString((uint)_value);
                else
                    return _value;
            }
            set
            {
                if (value == null)
                    _type = CellTypes.Null;
                else if (value is bool)
                    _type = CellTypes.Integer;
                else if (value is string)
                    _type = CellTypes.Text;
                else if (value is short)
                    _type = CellTypes.Integer;
                else if (value is int)
                    _type = CellTypes.Integer;
                else if (value is long)
                    _type = CellTypes.Integer;
                else if (value is Single)
                    _type = CellTypes.Float;
                else if (value is double)
                    _type = CellTypes.Float;
                else if (value is decimal)
                    _type = CellTypes.Float;
                else if (value is DateTime)
                {
                    value = ((DateTime)value).ToOADate();
                    _type = CellTypes.Float;
                }
                else
                    throw new NotSupportedException(string.Format("values of type {0}", value.GetType().Name));

                if (_type == CellTypes.Text && (value as string).Length > BIFF8.MaxCharactersPerCell)
                    throw new Exception(
                        string.Format("Text in Cell Row {0} Col {1} is longer than maximum allowed {2}", Row, Column,
                                      BIFF8.MaxCharactersPerCell));

                if (_type == CellTypes.Text && _worksheet.Document.Workbook.ShareStrings)
                    _value = _worksheet.Document.Workbook.SharedStringTable.Add((string)value);
                else
                    _value = value;
            }
        }

        internal XF ExtendedFormat
        {
            get
            {
                XF xf;
                if (_xfIdx == -1)
                    xf = _worksheet.Document.Workbook.XFs.DefaultUserXF;
                else
                    xf = _worksheet.Document.Workbook.XFs[_xfIdx];
                xf.Target = this;
                return xf;
            }
            set { _xfIdx = (value == null ? -1 : value.Id); }
        }

        internal void SetXfIndex(int xfIndex)
        {
            _xfIdx = xfIndex;
        }

        /// <summary>
        /// Gets or sets the Font for this Cell.
        /// </summary>
        public Font Font
        {
            get { return ExtendedFormat.Font; }
            set
            {
                XF xf = ExtendedFormat;
                xf.Font = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets the Format for this Cell.
        /// </summary>
        public string Format
        {
            get { return ExtendedFormat.Format; }
            set
            {
                XF xf = ExtendedFormat;
                xf.Format = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets the Parent Style XF (absent on Style XF's).
        /// </summary>
        public Style Style
        {
            get { return ExtendedFormat.Style; }
            set
            {
                XF xf = ExtendedFormat;
                xf.Style = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets the HorizontalAlignments value for this Cell.
        /// </summary>
        public HorizontalAlignments HorizontalAlignment
        {
            get { return ExtendedFormat.HorizontalAlignment; }
            set
            {
                XF xf = ExtendedFormat;
                xf.HorizontalAlignment = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets whether Text is wrapped at right border.
        /// </summary>
        public bool TextWrapRight
        {
            get { return ExtendedFormat.TextWrapRight; }
            set
            {
                XF xf = ExtendedFormat;
                xf.TextWrapRight = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets the VerticalAlignments value for this Cell.
        /// </summary>
        public VerticalAlignments VerticalAlignment
        {
            get { return ExtendedFormat.VerticalAlignment; }
            set
            {
                XF xf = ExtendedFormat;
                xf.VerticalAlignment = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets the Text rotation angle for this Cell (-360 or 0?) to 360.
        /// </summary>
        public short Rotation
        {
            get { return ExtendedFormat.Rotation; }
            set
            {
                XF xf = ExtendedFormat;
                xf.Rotation = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets the IndentLevel for this Cell.
        /// </summary>
        public ushort IndentLevel
        {
            get { return ExtendedFormat.IndentLevel; }
            set
            {
                XF xf = ExtendedFormat;
                xf.IndentLevel = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets whether to Shrink content to fit into cell for this Cell.
        /// </summary>
        public bool ShrinkToCell
        {
            get { return ExtendedFormat.ShrinkToCell; }
            set
            {
                XF xf = ExtendedFormat;
                xf.ShrinkToCell = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets the TextDirections value for this Cell (BIFF8X only).
        /// </summary>
        public TextDirections TextDirection
        {
            get { return ExtendedFormat.TextDirection; }
            set
            {
                XF xf = ExtendedFormat;
                xf.TextDirection = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets whether this Cell is locked.
        /// </summary>
        public bool Locked
        {
            get { return ExtendedFormat.CellLocked; }
            set
            {
                XF xf = ExtendedFormat;
                xf.CellLocked = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets whether this Cell's Formula is hidden
        /// </summary>
        public bool FormulaHidden
        {
            get { return ExtendedFormat.FormulaHidden; }
            set
            {
                XF xf = ExtendedFormat;
                xf.FormulaHidden = value;
                _xfIdx = xf.Id;
            }
        }

        /*        /// <summary>
                /// Gets or sets whether this XF is a Style XF.
                /// </summary>
                public bool IsStyleXF
                {
                    get { throw new NotImplementedException(); }
                    set { throw new NotImplementedException(); }
                }*/

        //TODO: Should UseNumber, UseFont, etc., be exposed to the user? or calculated internally
        /// <summary>
        /// Gets or sets Flag for number format.
        /// </summary>
        public bool UseNumber
        {
            get { return ExtendedFormat.UseNumber; }
            set
            {
                XF xf = ExtendedFormat;
                xf.UseNumber = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Flag for font.
        /// </summary>
        public bool UseFont
        {
            get { return ExtendedFormat.UseFont; }
            set
            {
                XF xf = ExtendedFormat;
                xf.UseFont = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Flag for horizontal and vertical alignment, text wrap, indentation, 
        /// orientation, rotation, and text direction.
        /// </summary>
        public bool UseMisc
        {
            get { return ExtendedFormat.UseMisc; }
            set
            {
                XF xf = ExtendedFormat;
                xf.UseMisc = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Flag for border lines.
        /// </summary>
        public bool UseBorder
        {
            get { return ExtendedFormat.UseBorder; }
            set
            {
                XF xf = ExtendedFormat;
                xf.UseBorder = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Flag for background area style.
        /// </summary>
        public bool UseBackground
        {
            get { return ExtendedFormat.UseBackground; }
            set
            {
                XF xf = ExtendedFormat;
                xf.UseBackground = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Flag for protection (cell locked and formula hidden).
        /// </summary>
        public bool UseProtection
        {
            get { return ExtendedFormat.UseProtection; }
            set
            {
                XF xf = ExtendedFormat;
                xf.UseProtection = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Left line style.
        /// </summary>
        public ushort LeftLineStyle
        {
            get { return ExtendedFormat.LeftLineStyle; }
            set
            {
                XF xf = ExtendedFormat;
                xf.LeftLineStyle = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Right line style.
        /// </summary>
        public ushort RightLineStyle
        {
            get { return ExtendedFormat.RightLineStyle; }
            set
            {
                XF xf = ExtendedFormat;
                xf.RightLineStyle = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Top line style.
        /// </summary>
        public ushort TopLineStyle
        {
            get { return ExtendedFormat.TopLineStyle; }
            set
            {
                XF xf = ExtendedFormat;
                xf.TopLineStyle = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Bottom line style.
        /// </summary>
        public ushort BottomLineStyle
        {
            get { return ExtendedFormat.BottomLineStyle; }
            set
            {
                XF xf = ExtendedFormat;
                xf.BottomLineStyle = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Colour for left line.
        /// </summary>
        public Color LeftLineColor
        {
            get { return ExtendedFormat.LeftLineColor; }
            set
            {
                XF xf = ExtendedFormat;
                xf.LeftLineColor = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Colour for right line.
        /// </summary>
        public Color RightLineColor
        {
            get { return ExtendedFormat.RightLineColor; }
            set
            {
                XF xf = ExtendedFormat;
                xf.RightLineColor = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Diagonal line from top left to right bottom.
        /// </summary>
        public bool DiagonalDescending
        {
            get { return ExtendedFormat.DiagonalDescending; }
            set
            {
                XF xf = ExtendedFormat;
                xf.DiagonalDescending = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Diagonal line from bottom left to top right.  Lines won't show up unless
        /// DiagonalLineStyle (and color - defaults to black, though) is set.
        /// </summary>
        public bool DiagonalAscending
        {
            get { return ExtendedFormat.DiagonalAscending; }
            set
            {
                XF xf = ExtendedFormat;
                xf.DiagonalAscending = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Colour for top line.
        /// </summary>
        public Color TopLineColor
        {
            get { return ExtendedFormat.TopLineColor; }
            set
            {
                XF xf = ExtendedFormat;
                xf.TopLineColor = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Colour for bottom line.
        /// </summary>
        public Color BottomLineColor
        {
            get { return ExtendedFormat.BottomLineColor; }
            set
            {
                XF xf = ExtendedFormat;
                xf.BottomLineColor = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Colour for diagonal line.
        /// </summary>
        public Color DiagonalLineColor
        {
            get { return ExtendedFormat.DiagonalLineColor; }
            set
            {
                XF xf = ExtendedFormat;
                xf.DiagonalLineColor = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Diagonal line style.
        /// </summary>
        public LineStyle DiagonalLineStyle
        {
            get { return ExtendedFormat.DiagonalLineStyle; }
            set
            {
                XF xf = ExtendedFormat;
                xf.DiagonalLineStyle = value;
                _xfIdx = xf.Id;
            }
        }

        //TODO: Create Standard Fill Pattern constants
        /// <summary>
        /// Gets or sets Fill pattern.
        /// </summary>
        public ushort Pattern
        {
            get { return ExtendedFormat.Pattern; }
            set
            {
                XF xf = ExtendedFormat;
                xf.Pattern = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Colour for pattern.
        /// </summary>
        public Color PatternColor
        {
            get { return ExtendedFormat.PatternColor; }
            set
            {
                XF xf = ExtendedFormat;
                xf.PatternColor = value;
                _xfIdx = xf.Id;
            }
        }

        /// <summary>
        /// Gets or sets Colour for pattern background.
        /// </summary>
        public Color PatternBackgroundColor
        {
            get { return ExtendedFormat.PatternBackgroundColor; }
            set
            {
                XF xf = ExtendedFormat;
                xf.PatternBackgroundColor = value;
                _xfIdx = xf.Id;
            }
        }

        internal Bytes Bytes
        {
            get
            {
                if (_xfIdx < 0)
                    _xfIdx = ExtendedFormat.Id;

                switch (Type)
                {
                    //NOTE: Optimization - cache the values for all these (RK, NUMBER)
                    case CellTypes.Integer:
                        return RK(false);
                    case CellTypes.Float:
                        {
                            double dbl = Convert.ToDouble(_value);
                            bool div100 = false;
                            double tmp = dbl;
                            tmp *= 10; //doing a single (tmp *= 100) operation introduces float inaccuracies (1.1 * 10 * 10 = 110.0, whereas 1.1 * 100 = 110.000000000001)
                            tmp *= 10;
                            if (Math.Floor(tmp) == tmp)
                                div100 = true;

                            //per excelfileformat.pdf sec.5.83 (RK):
                            //If a floating-point value cannot be encoded to an RK value, a NUMBER
                            //record (5.69) will be written.
                            Bytes rk = RK(div100);
                            if (DecodeRKFloat(rk.GetBits(), div100) == dbl)
                                return rk;
                            else
                                return NUMBER();
                        }
                    case CellTypes.Text:
                        {
                            if (_value is string)
                                return LABEL();
                            else
                                return LABELSST();
                        }

                    case CellTypes.Formula:
                    case CellTypes.Error:
                        throw new NotSupportedException(string.Format("CellType {0}", Type));

                    case CellTypes.Null:
                        return BLANK();

                    default:
                        throw new Exception(string.Format("unexpected CellTypes {0}", Type));
                }
            }
        }

        private Bytes BLANK()
        {
            Bytes blank = new Bytes();

            //Index to row
            blank.Append(BitConverter.GetBytes((ushort)(Row - 1)));

            //Index to column
            blank.Append(BitConverter.GetBytes((ushort)(Column - 1)));

            //Index to XF record
            blank.Append(BitConverter.GetBytes((ushort)_xfIdx));

            return Record.GetBytes(RID.BLANK, blank);
        }

        private Bytes LABEL()
        {
            Bytes label = new Bytes();

            label.Append(LABELBase());

            //Unicode string, 16-bit string length
            label.Append(XlsDocument.GetUnicodeString((string)Value ?? string.Empty, 16));

            return Record.GetBytes(RID.LABEL, label);
        }

        private Bytes LABELSST()
        {
            Bytes labelsst = new Bytes();

            labelsst.Append(LABELBase());

            //Index of string value in Shared String Table
            labelsst.Append(BitConverter.GetBytes((uint)_value));

            return Record.GetBytes(RID.LABELSST, labelsst);
        }

        private Bytes LABELBase()
        {
            Bytes labelBase = new Bytes();

            //Index to row
            labelBase.Append(BitConverter.GetBytes((ushort)(Row - 1)));

            //Index to column
            labelBase.Append(BitConverter.GetBytes((ushort)(Column - 1)));

            //Index to XF record
            labelBase.Append(BitConverter.GetBytes((ushort)_xfIdx));

            return labelBase;
        }

        private Bytes RK(bool trueFalse)
        {
            Bytes rk = new Bytes();

            //Index to row
            rk.Append(BitConverter.GetBytes((ushort)(Row - 1)));

            //Index to column
            rk.Append(BitConverter.GetBytes((ushort)(Column - 1)));

            //Index to XF record
            rk.Append(BitConverter.GetBytes((ushort)_xfIdx));

            //RK Value
            if (Type == CellTypes.Integer)
                rk.Append(RKIntegerValue(Value, trueFalse));
            else if (Type == CellTypes.Float)
                rk.Append(RKDecimalValue(Value, trueFalse));

            return Record.GetBytes(RID.RK, rk);
        }

        private Bytes NUMBER()
        {
            double value = Convert.ToDouble(Value);

            Bytes number = new Bytes();

            //Index to row
            number.Append(BitConverter.GetBytes((ushort)(Row - 1)));

            //Index to column
            number.Append(BitConverter.GetBytes((ushort)(Column - 1)));

            //Index to XF record
            number.Append(BitConverter.GetBytes((ushort)_xfIdx));

            //NUMBER Value
            number.Append(NUMBERVal(value));

            return Record.GetBytes(RID.NUMBER, number);
        }

        private static Bytes RKDecimalValue(object val, bool div100)
        {
            double rk = Convert.ToDouble(val);

            if (div100)
            {
                rk *= 10; //doing a single (tmp *= 100) operation introduces float inaccuracies (1.1 * 10 * 10 = 110.0, whereas 1.1 * 100 = 110.000000000001)
                rk *= 10;
            }

            Bytes bytes = new Bytes(BitConverter.GetBytes(rk));
            List<bool> bitList = new List<bool>();
            bitList.Add(div100);
            bitList.Add(false);
            bitList.AddRange(bytes.GetBits().Get(34, 30).Values);

            return (new Bytes.Bits(bitList.ToArray())).GetBytes();
        }

        private static Bytes RKIntegerValue(object val, bool div100)
        {
            int rk = Convert.ToInt32(val);
            if (rk < -536870912 || rk >= 536870912)
                throw new ArgumentOutOfRangeException("val", string.Format("{0}: must be between -536870912 and 536870911", rk));

            unchecked
            {
                rk = rk << 2;
            }

            if (div100)
                rk += 1;
            rk += 2;

            byte[] bytes = BitConverter.GetBytes(rk);

            return new Bytes(bytes);
        }

        private static Bytes NUMBERVal(double val)
        {
            return new Bytes(BitConverter.GetBytes(val));
        }

        #region IXFTarget Members

        ///<summary>
        /// (For internal use only) - Updates this Cell's XF id from the provided XF.
        ///</summary>
        ///<param name="fromXF">The XF from which to calculate this Cell's XF id.</param>
        public void UpdateId(XF fromXF)
        {
            _xfIdx = fromXF.Id;
        }

        #endregion

        internal void SetValue(byte[] rid, Bytes data)
        {
            if (rid == RID.RK)
                DecodeRK(data);
            else if (rid == RID.LABEL)
                DecodeLABEL(data);
            else if (rid == RID.LABELSST)
                DecodeLABELSST(data);
            else if (rid == RID.NUMBER)
                DecodeNUMBER(data);
            else
                throw new ApplicationException(string.Format("Unsupported RID {0}", RID.Name(rid)));
        }

        internal void SetFormula(Bytes data, Record stringRecord)
        {
            DecodeFORMULA(data, stringRecord);
        }

        private void DecodeFORMULA(Bytes data, Record stringRecord)
        {
            //TODO: Read in formula properties
            if (stringRecord != null)
                _value = UnicodeBytes.Read(stringRecord.Data, 16);
            _type = CellTypes.Formula;
        }

        private void DecodeNUMBER(Bytes data)
        {
            _value = BitConverter.ToDouble(data.ByteArray, 0);
            _type = CellTypes.Float;
        }

        private void DecodeLABELSST(Bytes data)
        {
            _value = BitConverter.ToUInt32(data.ByteArray, 0);
            _type = CellTypes.Text;
        }

        private void DecodeLABEL(Bytes data)
        {
            _value = UnicodeBytes.Read(data, 16);
            _type = CellTypes.Text;
        }

        private void DecodeRK(Bytes bytes)
        {
            Bytes.Bits bits = bytes.GetBits();
            bool div100 = bits.Values[0];
            bool isInt = bits.Values[1];
            if (isInt)
            {
                Value = DecodeRKInt(bits, div100);
                _type = (Value is Int32) ? CellTypes.Integer : CellTypes.Float;
            }
            else
            {
                Value = DecodeRKFloat(bits, div100);
                _type = CellTypes.Float;
            }
        }

        private static double DecodeRKFloat(Bytes.Bits bits, bool div100)
        {
            Bytes.Bits floatBits = bits.Get(2, 30);
            floatBits.Prepend(false); //right-shift to full 8 bytes
            floatBits.Prepend(false);
            byte[] floatBytes = new byte[8];
            floatBits.GetBytes().ByteArray.CopyTo(floatBytes, 4);
            byte[] double1Bytes = BitConverter.GetBytes((double)1);
            double val = BitConverter.ToDouble(floatBytes, 0);
            if (div100)
            {
                val /= 100.0;
            }
            return val;
        }

        private static object DecodeRKInt(Bytes.Bits bits, bool div100)
        {
            object val = bits.Get(2, 30).ToInt32();
            if (div100)
            {
                val = Convert.ToDouble(val) / 100.0;
            }
            return val;
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Holds the Row and Column indices for a Cell's Coordinate value.
    /// </summary>
    public struct CellCoordinate
    {
        /// <summary>
        /// Row index (1-based).
        /// </summary>
        public ushort Row;

        /// <summary>
        /// Column index (1-based).
        /// </summary>
        public ushort Column;

        /// <summary>
        /// Initializes a new instance of the CellCoordinate struct with the given values.
        /// </summary>
        /// <param name="row">Row index (1-based).</param>
        /// <param name="column">Column index (1-based).</param>
        public CellCoordinate(ushort row, ushort column)
        {
            Row = row;
            Column = column;
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Represents and manages a collection of Cells for a Worksheet.
    /// </summary>
    public class Cells
    {
        //		private Doc _doc;
        private readonly Worksheet _worksheet;

        private ushort _cellCount = 0;
        private ushort _minRow = 0;
        private ushort _maxRow = 0;
        private ushort _minCol = 0;
        private ushort _maxCol = 0;

        /// <summary>
        /// Initializes a new instance of the Cells collection class for the given Worksheet.
        /// </summary>
        /// <param name="worksheet">The parent Worksheet object for this new Cells collection.</param>
        public Cells(Worksheet worksheet)
        {
            //			_doc = doc;
            _worksheet = worksheet;
        }

        internal Cell Add(ushort cellRow, ushort cellColumn)
        {
            Cell cell = new Cell(_worksheet);
            bool haveCell = false;

            if (cellColumn < 1)
                throw new ArgumentOutOfRangeException("cellColumn", string.Format("{0} must be >= 1", cellColumn));
            else if (cellColumn > BIFF8.MaxCols)
                throw new ArgumentOutOfRangeException("cellColumn", string.Format("{0} cellColumn must be <= {1}", cellColumn, BIFF8.MaxCols));

            if (cellRow < 1)
                throw new ArgumentOutOfRangeException("cellRow", string.Format("{0} must be >= 1", cellColumn));
            else if (cellRow > BIFF8.MaxRows)
                throw new ArgumentOutOfRangeException("cellRow", string.Format("{0} cellRow must be <= {1}", cellRow, BIFF8.MaxRows));

            if (_worksheet.Rows.RowExists(cellRow))
            {
                if (_worksheet.Rows[cellRow].CellExists(cellColumn))
                {
                    cell = _worksheet.Rows[cellRow].CellAtCol(cellColumn);
                    haveCell = true;
                }
            }
            else
                _worksheet.Rows.AddRow(cellRow);

            if (haveCell)
                return cell;

            cell.Coordinate = new CellCoordinate(cellRow, cellColumn);

            if (_minRow == 0)
            {
                _minRow = cellRow;
                _minCol = cellColumn;
                _maxRow = cellRow;
                _maxCol = cellColumn;
            }
            else
            {
                if (cellRow < _minRow)
                    _minRow = cellRow;
                else if (cellRow > _maxRow)
                    _maxRow = cellRow;

                if (cellColumn < _minCol)
                    _minCol = cellColumn;
                else if (cellColumn > _maxCol)
                    _maxCol = cellColumn;
            }

            _worksheet.Rows[cellRow].AddCell(cell);
            _cellCount++;

            return cell;
        }

        /// <summary>
        /// Adds a new Cell to the Cells collection with the given Row, Column and
        /// Value.  If a Cell already exists with the given row and column, it is
        /// overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row index for the new Cell.</param>
        /// <param name="cellColumn">1-based Column index for the new Cell.</param>
        /// <param name="cellValue">Value for the new Cell.</param>
        /// <returns>The newly added Cell with the given Row, Column and Value.</returns>
        public Cell Add(int cellRow, int cellColumn, object cellValue)
        {
            Util.ValidateUShort(cellRow, "cellRow");
            Util.ValidateUShort(cellColumn, "cellColumn");
            return Add((ushort)cellRow, (ushort)cellColumn, cellValue);
        }

        /// <summary>
        /// OBSOLETE - Use Add(int, int, object) instead.  Adds a new Cell to the 
        /// Cells collection with the given Row, Column and Value.  If a Cell 
        /// already exists with the given row and column, it is overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row index for the new Cell.</param>
        /// <param name="cellColumn">1-based Column index for the new Cell.</param>
        /// <param name="cellValue">Value for the new Cell.</param>
        /// <returns>The newly added Cell with the given Row, Column and Value.</returns>
        [Obsolete]
        public Cell AddValueCell(int cellRow, int cellColumn, object cellValue)
        {
            return Add(cellRow, cellColumn, cellValue);
        }

        /// <summary>
        /// Adds a new Cell to the Cells collection with the given Row, Column and
        /// Value.  If a Cell already exists with the given row and column, it is
        /// overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row index for the new Cell.</param>
        /// <param name="cellColumn">1-based Column index for the new Cell.</param>
        /// <param name="cellValue">Value for the new Cell.</param>
        /// <returns>The newly added Cell with the given Row, Column and Value.</returns>
        public Cell Add(ushort cellRow, ushort cellColumn, object cellValue)
        {
            Cell cell = Add(cellRow, cellColumn);
            cell.Value = cellValue;
            return cell;
        }

        /// <summary>
        /// OBSOLETE - Use Add(ushort, ushort, object) instead.  Adds a new Cell to 
        /// the Cells collection with the given Row, Column and Value.  If a Cell 
        /// already exists with the given row and column, it is overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row index for the new Cell.</param>
        /// <param name="cellColumn">1-based Column index for the new Cell.</param>
        /// <param name="cellValue">Value for the new Cell.</param>
        /// <returns>The newly added Cell with the given Row, Column and Value.</returns>
        [Obsolete]
        public Cell AddValueCell(ushort cellRow, ushort cellColumn, object cellValue)
        {
            return Add(cellRow, cellColumn, cellValue);
        }

        /// <summary>
        /// Adds a new Cell to the Cells collection with the given Row, Column, Value
        /// and XF (style).  If a Cell already exists with the given row and column,
        /// it is overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row of new Cell.</param>
        /// <param name="cellColumn">1-based Column of new Cell.</param>
        /// <param name="cellValue">Value of new Cell.</param>
        /// <param name="xf">An Xf object describing the style of the cell.</param>
        /// <returns>The newly added Cell with the given Row, Column, Value and Style.</returns>
        public Cell Add(int cellRow, int cellColumn, object cellValue, XF xf)
        {
            Util.ValidateUShort(cellRow, "cellRow");
            Util.ValidateUShort(cellColumn, "cellColumn");
            return Add((ushort)cellRow, (ushort)cellColumn, cellValue, xf);
        }

        /// <summary>
        /// OBSOLETE - Use Add(int, int, object, XF) instead.  Adds a new Cell to the 
        /// Cells collection with the given Row, Column, Value and XF (style).  If a 
        /// Cell already exists with the given row and column, it is overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row of new Cell.</param>
        /// <param name="cellColumn">1-based Column of new Cell.</param>
        /// <param name="cellValue">Value of new Cell.</param>
        /// <param name="xf">An Xf object describing the style of the cell.</param>
        /// <returns>The newly added Cell with the given Row, Column, Value and Style.</returns>
        [Obsolete]
        public Cell AddValueCellXF(int cellRow, int cellColumn, object cellValue, XF xf)
        {
            return Add(cellRow, cellColumn, cellValue, xf);
        }

        /// <summary>
        /// Adds a new Cell to the Cells collection with the given Row, Column, Value
        /// and XF (style).  If a Cell already exists with the given row and column,
        /// it is overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row of new Cell.</param>
        /// <param name="cellColumn">1-based Column of new Cell.</param>
        /// <param name="cellValue">Value of new Cell.</param>
        /// <param name="xf">An Xf object describing the style of the cell.</param>
        /// <returns>The newly added Cell with the given Row, Column, Value and Style.</returns>
        public Cell Add(ushort cellRow, ushort cellColumn, object cellValue, XF xf)
        {
            Cell cell = Add(cellRow, cellColumn, cellValue);
            cell.ExtendedFormat = xf;
            return cell;
        }

        /// <summary>
        /// OBSOLETE - Use Add(ushort, ushort, object, XF) instead.  Adds a new Cell 
        /// to the Cells collection with the given Row, Column, Value and XF (style).  
        /// If a Cell already exists with the given row and column, it is overwritten.
        /// </summary>
        /// <param name="cellRow">1-based Row of new Cell.</param>
        /// <param name="cellColumn">1-based Column of new Cell.</param>
        /// <param name="cellValue">Value of new Cell.</param>
        /// <param name="xf">An Xf object describing the style of the cell.</param>
        /// <returns>The newly added Cell with the given Row, Column, Value and Style.</returns>
        [Obsolete]
        public Cell AddValueCellXF(ushort cellRow, ushort cellColumn, object cellValue, XF xf)
        {
            return Add(cellRow, cellColumn, cellValue, xf);
        }

        /// <summary>
        /// Merges cells within the defined range of Rows and Columns.  The ranges are
        /// verified not to overlap with any previously defined Merge areas.  NOTE 
        /// Values and formatting in all cells other than the first in the range 
        /// (scanning left to right, top to bottom) will be lost.
        /// </summary>
        /// <param name="rowMin">The first index in the range of Rows to merge.</param>
        /// <param name="rowMax">The last index in the range of Rows to merge.</param>
        /// <param name="colMin">The first index in the range of Columns to merge.</param>
        /// <param name="colMax">The last index in the range of Columns to merge.</param>
        public void Merge(int rowMin, int rowMax, int colMin, int colMax)
        {
            MergeArea mergeArea = new MergeArea(rowMin, rowMax, colMin, colMax);
            _worksheet.AddMergeArea(mergeArea);
        }

        /// <summary>
        /// Gets the count of cells in this collection (only counts cells with assigned
        /// values, styles or properties - not blank/unused cells).
        /// </summary>
        public ushort Count
        {
            get { return _cellCount; }
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// The different types of Cell values.
    /// </summary>
    public enum CellTypes
    {
        /// <summary>Error</summary>
        Error,

        /// <summary>Null</summary>
        Null,

        /// <summary>Integer</summary>
        Integer,

        /// <summary>Text</summary>
        Text,

        /// <summary>Floating Point Number</summary>
        Float,

        /// <summary>Formula</summary>
        Formula
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Different Character Sets supported by Excel.
    /// </summary>
    public enum CharacterSets : byte
    {
        /// <summary>Default - ANSI Latin</summary>
        Default = ANSILatin,

        /// <summary>ANSI Latin</summary>
        ANSILatin = 0x00,

        /// <summary>System Default</summary>
        SystemDefault = 0x01,

        /// <summary>Symbol</summary>
        Symbol = 0x02,

        /// <summary>Apple Roman</summary>
        AppleRoman = 0x4D,

        /// <summary>ANSI Japanese Shift JIS</summary>
        ANSIJapaneseShiftJIS = 0x80,

        /// <summary>ANSI Korean Hangul</summary>
        ANSIKoreanHangul = 0x81,

        /// <summary>ANSI Korean Johab</summary>
        ANSIKoreanJohab = 0x82,

        /// <summary>ANSI Chinese Simplified JBK</summary>
        ANSIChineseSimplifiedJBK = 0x86,

        /// <summary>ANSI Chinese Traditional BIG5</summary>
        ANSIChineseTraditionalBIG5 = 0x88,

        /// <summary>ANSI Greek</summary>
        ANSIGreek = 0xA1,

        /// <summary>ANSI Turkish</summary>
        ANSITurkish = 0xA2,

        /// <summary>ANSI Vietnamese</summary>
        ANSIVietnamese = 0xA3,

        /// <summary>ANSI Hebrew</summary>
        ANSIHebrew = 0xB1,

        /// <summary>ANSI Arabic</summary>
        ANSIArabic = 0xB2,

        /// <summary>ANSI Baltic</summary>
        ANSIBaltic = 0xBA,

        /// <summary>ANSI Cyrillic</summary>
        ANSICyrillic = 0xCC,

        /// <summary>ANSI Thai</summary>
        ANSIThai = 0xDE,

        /// <summary>ANSI Latin II Central European</summary>
        ANSILatinIICentralEuropean = 0xEE,

        /// <summary>OEM Latin I</summary>
        OEMLatinI = 0xFF
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Represents an RGB color value.  Use the values in Colors.  Custom colors are not yet supported.
    /// </summary>
    public class Color : ICloneable
    {
        internal byte Red;
        internal byte Green;
        internal byte Blue;
        internal ushort? Id;

        internal Color(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Id = null;
        }

        internal Color(ushort id)
        {
            Red = 0x00;
            Green = 0x00;
            Blue = 0x00;
            Id = id;
        }

        /// <summary>
        /// Determines whether this Color is equal to the specifed Color.
        /// </summary>
        /// <param name="obj">The Color to compare to this Color.</param>
        /// <returns>true if this Color is Equal to that Color, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            Color that = (Color)obj;
            if (this.Id != null || that.Id != null)
                return this.Id == that.Id;
            else
                return (this.Red == that.Red &&
                        this.Green == that.Green &&
                        this.Blue == that.Blue);
        }

        /// <summary>
        /// Returns a new Color instance Equal to this Color.
        /// </summary>
        /// <returns>A new Color instance Equal to this Color.</returns>
        public object Clone()
        {
            Color clone = new Color(Red, Green, Blue);
            clone.Id = Id;
            return clone;
        }
    }
}
namespace Core.MyXls
{
    ///<summary>
    /// Provides named references to all colors in the standard (BIFF8) Excel
    /// color palette.
    ///</summary>
    public static class Colors
    {
        #region (System?) EGA Colors (don't use - they seem to break Excel's Format dialog)

        /// <summary>EGA Black (use Black instead)</summary>
        public static readonly Color EgaBlack = new Color(0x00, 0x00, 0x00);

        /// <summary>EGA White (use White instead)</summary>
        public static readonly Color EgaWhite = new Color(0xFF, 0xFF, 0xFF);

        /// <summary>EGA Red (use Red instead)</summary>
        public static readonly Color EgaRed = new Color(0xFF, 0x00, 0x00);

        /// <summary>EGA Green (use Green instead)</summary>
        public static readonly Color EgaGreen = new Color(0x00, 0xFF, 0x00);

        /// <summary>EGA Blue (use Blue instead)</summary>
        public static readonly Color EgaBlue = new Color(0x00, 0x00, 0xFF);

        /// <summary>EGA Yellow (use Yellow instead)</summary>
        public static readonly Color EgaYellow = new Color(0xFF, 0xFF, 0x00);

        /// <summary>EGA Magenta (use Magenta instead)</summary>
        public static readonly Color EgaMagenta = new Color(0xFF, 0x00, 0xFF);

        /// <summary>EGA Cyan (use Cyan instead)</summary>
        public static readonly Color EgaCyan = new Color(0x00, 0xFF, 0xFF);

        #endregion

        #region Default Palette Colors

        /// <summary>Default Palette Color Index 0x08 (#000000)</summary>
        public static readonly Color Default08 = new Color(0x00, 0x00, 0x00);

        /// <summary>Default Palette Color Index 0x09 (#FFFFFF)</summary>
        public static readonly Color Default09 = new Color(0xFF, 0xFF, 0xFF);

        /// <summary>Default Palette Color Index 0x0A (#FF0000)</summary>
        public static readonly Color Default0A = new Color(0xFF, 0x00, 0x00);

        /// <summary>Default Palette Color Index 0x0B (#00FF00)</summary>
        public static readonly Color Default0B = new Color(0x00, 0xFF, 0x00);

        /// <summary>Default Palette Color Index 0x0C (#0000FF)</summary>
        public static readonly Color Default0C = new Color(0x00, 0x00, 0xFF);

        /// <summary>Default Palette Color Index 0x0D (#FFFF00)</summary>
        public static readonly Color Default0D = new Color(0xFF, 0xFF, 0x00);

        /// <summary>Default Palette Color Index 0x0E (#FF00FF)</summary>
        public static readonly Color Default0E = new Color(0xFF, 0x00, 0xFF);

        /// <summary>Default Palette Color Index 0x0F (#00FFFF)</summary>
        public static readonly Color Default0F = new Color(0x00, 0xFF, 0xFF);

        /// <summary>Default Palette Color Index 0x10 (#800000)</summary>
        public static readonly Color Default10 = new Color(0x80, 0x00, 0x00);

        /// <summary>Default Palette Color Index 0x11 (#008000)</summary>
        public static readonly Color Default11 = new Color(0x00, 0x80, 0x00);

        /// <summary>Default Palette Color Index 0x12 (#000080)</summary>
        public static readonly Color Default12 = new Color(0x00, 0x00, 0x80);

        /// <summary>Default Palette Color Index 0x13 (#808000)</summary>
        public static readonly Color Default13 = new Color(0x80, 0x80, 0x00);

        /// <summary>Default Palette Color Index 0x14 (#800080)</summary>
        public static readonly Color Default14 = new Color(0x80, 0x00, 0x80);

        /// <summary>Default Palette Color Index 0x15 (#008080)</summary>
        public static readonly Color Default15 = new Color(0x00, 0x80, 0x80);

        /// <summary>Default Palette Color Index 0x16 (#C0C0C0)</summary>
        public static readonly Color Default16 = new Color(0xC0, 0xC0, 0xC0);

        /// <summary>Default Palette Color Index 0x17 (#808080)</summary>
        public static readonly Color Default17 = new Color(0x80, 0x80, 0x80);

        /// <summary>Default Palette Color Index 0x18 (#9999FF)</summary>
        public static readonly Color Default18 = new Color(0x99, 0x99, 0xFF);

        /// <summary>Default Palette Color Index 0x19 (#993366)</summary>
        public static readonly Color Default19 = new Color(0x99, 0x33, 0x66);

        /// <summary>Default Palette Color Index 0x1A (#FFFFCC)</summary>
        public static readonly Color Default1A = new Color(0xFF, 0xFF, 0xCC);

        /// <summary>Default Palette Color Index 0x1B (#CCFFFF)</summary>
        public static readonly Color Default1B = new Color(0xCC, 0xFF, 0xFF);

        /// <summary>Default Palette Color Index 0x1C (#660066)</summary>
        public static readonly Color Default1C = new Color(0x66, 0x00, 0x66);

        /// <summary>Default Palette Color Index 0x1D (#FF8080)</summary>
        public static readonly Color Default1D = new Color(0xFF, 0x80, 0x80);

        /// <summary>Default Palette Color Index 0x1E (#0066CC)</summary>
        public static readonly Color Default1E = new Color(0x00, 0x66, 0xCC);

        /// <summary>Default Palette Color Index 0x1F (#CCCCFF)</summary>
        public static readonly Color Default1F = new Color(0xCC, 0xCC, 0xFF);

        /// <summary>Default Palette Color Index 0x20 (#000080)</summary>
        public static readonly Color Default20 = new Color(0x00, 0x00, 0x80);

        /// <summary>Default Palette Color Index 0x21 (#FF00FF)</summary>
        public static readonly Color Default21 = new Color(0xFF, 0x00, 0xFF);

        /// <summary>Default Palette Color Index 0x22 (#FFFF00)</summary>
        public static readonly Color Default22 = new Color(0xFF, 0xFF, 0x00);

        /// <summary>Default Palette Color Index 0x23 (#00FFFF)</summary>
        public static readonly Color Default23 = new Color(0x00, 0xFF, 0xFF);

        /// <summary>Default Palette Color Index 0x24 (#800080)</summary>
        public static readonly Color Default24 = new Color(0x80, 0x00, 0x80);

        /// <summary>Default Palette Color Index 0x25 (#800000)</summary>
        public static readonly Color Default25 = new Color(0x80, 0x00, 0x00);

        /// <summary>Default Palette Color Index 0x26 (#008080)</summary>
        public static readonly Color Default26 = new Color(0x00, 0x80, 0x80);

        /// <summary>Default Palette Color Index 0x27 (#0000FF)</summary>
        public static readonly Color Default27 = new Color(0x00, 0x00, 0xFF);

        /// <summary>Default Palette Color Index 0x28 (#00CCFF)</summary>
        public static readonly Color Default28 = new Color(0x00, 0xCC, 0xFF);

        /// <summary>Default Palette Color Index 0x29 (#CCFFFF)</summary>
        public static readonly Color Default29 = new Color(0xCC, 0xFF, 0xFF);

        /// <summary>Default Palette Color Index 0x2A (#CCFFCC)</summary>
        public static readonly Color Default2A = new Color(0xCC, 0xFF, 0xCC);

        /// <summary>Default Palette Color Index 0x2B (#FFFF99)</summary>
        public static readonly Color Default2B = new Color(0xFF, 0xFF, 0x99);

        /// <summary>Default Palette Color Index 0x2C (#99CCFF)</summary>
        public static readonly Color Default2C = new Color(0x99, 0xCC, 0xFF);

        /// <summary>Default Palette Color Index 0x2D (#FF99CC)</summary>
        public static readonly Color Default2D = new Color(0xFF, 0x99, 0xCC);

        /// <summary>Default Palette Color Index 0x2E (#CC99FF)</summary>
        public static readonly Color Default2E = new Color(0xCC, 0x99, 0xFF);

        /// <summary>Default Palette Color Index 0x2F (#FFCC99)</summary>
        public static readonly Color Default2F = new Color(0xFF, 0xCC, 0x99);

        /// <summary>Default Palette Color Index 0x30 (#3366FF)</summary>
        public static readonly Color Default30 = new Color(0x33, 0x66, 0xFF);

        /// <summary>Default Palette Color Index 0x31 (#33CCCC)</summary>
        public static readonly Color Default31 = new Color(0x33, 0xCC, 0xCC);

        /// <summary>Default Palette Color Index 0x32 (#99CC00)</summary>
        public static readonly Color Default32 = new Color(0x99, 0xCC, 0x00);

        /// <summary>Default Palette Color Index 0x33 (#FFCC00)</summary>
        public static readonly Color Default33 = new Color(0xFF, 0xCC, 0x00);

        /// <summary>Default Palette Color Index 0x34 (#FF9900)</summary>
        public static readonly Color Default34 = new Color(0xFF, 0x99, 0x00);

        /// <summary>Default Palette Color Index 0x35 (#FF6600)</summary>
        public static readonly Color Default35 = new Color(0xFF, 0x66, 0x00);

        /// <summary>Default Palette Color Index 0x36 (#666699)</summary>
        public static readonly Color Default36 = new Color(0x66, 0x66, 0x99);

        /// <summary>Default Palette Color Index 0x37 (#969696)</summary>
        public static readonly Color Default37 = new Color(0x96, 0x96, 0x96);

        /// <summary>Default Palette Color Index 0x38 (#003366)</summary>
        public static readonly Color Default38 = new Color(0x00, 0x33, 0x66);

        /// <summary>Default Palette Color Index 0x39 (#339966)</summary>
        public static readonly Color Default39 = new Color(0x33, 0x99, 0x66);

        /// <summary>Default Palette Color Index 0x3A (#003300)</summary>
        public static readonly Color Default3A = new Color(0x00, 0x33, 0x00);

        /// <summary>Default Palette Color Index 0x3B (#333300)</summary>
        public static readonly Color Default3B = new Color(0x33, 0x33, 0x00);

        /// <summary>Default Palette Color Index 0x3C (#993300)</summary>
        public static readonly Color Default3C = new Color(0x99, 0x33, 0x00);

        /// <summary>Default Palette Color Index 0x3D (#993366)</summary>
        public static readonly Color Default3D = new Color(0x99, 0x33, 0x66);

        /// <summary>Default Palette Color Index 0x3E (#333399)</summary>
        public static readonly Color Default3E = new Color(0x33, 0x33, 0x99);

        /// <summary>Default Palette Color Index 0x3F (#333333)</summary>
        public static readonly Color Default3F = new Color(0x33, 0x33, 0x33);

        #endregion

        /// <summary>Black - Alias for Default08</summary>
        public static readonly Color Black = Default08;

        /// <summary>White - Alias for Default09</summary>
        public static readonly Color White = Default09;

        /// <summary>Red - Alias for Default0A</summary>
        public static readonly Color Red = Default0A;

        /// <summary>Green - Alias for Default0B</summary>
        public static readonly Color Green = Default0B;

        /// <summary>Blue - Alias for Default0C</summary>
        public static readonly Color Blue = Default0C;

        /// <summary>Yellow - Alias for Default0D</summary>
        public static readonly Color Yellow = Default0D;

        /// <summary>Magenta - Alias for Default0E</summary>
        public static readonly Color Magenta = Default0E;

        /// <summary>Cyan - Alias for Default0F</summary>
        public static readonly Color Cyan = Default0F;

        /// <summary>DarkRed - Alias for Default10</summary>
        public static readonly Color DarkRed = Default10;

        /// <summary>DarkGreen - Alias for Default11</summary>
        public static readonly Color DarkGreen = Default11;

        /// <summary>DarkBlue - Alias for Default12</summary>
        public static readonly Color DarkBlue = Default12;

        /// <summary>Olive - Alias for Default13</summary>
        public static readonly Color Olive = Default13;

        /// <summary>Purple - Alias for Default14</summary>
        public static readonly Color Purple = Default14;

        /// <summary>Teal - Alias for Default15</summary>
        public static readonly Color Teal = Default15;

        /// <summary>Silver - Alias for Default16</summary>
        public static readonly Color Silver = Default16;

        /// <summary>Grey - Alias for Default17</summary>
        public static readonly Color Grey = Default17;

        /// <summary>System window text colour for border lines (used in records XF, CF, and WINDOW2 (BIFF8 only))</summary>
        public static readonly Color SystemWindowTextColorForBorderLines = new Color(64);

        /// <summary>System window background colour for pattern background (used in records XF, and CF)</summary>
        public static readonly Color SystemWindowBackgroundColorForPatternBackground = new Color(65);

        /// <summary>System face colour (dialogue background colour)</summary>
        public static readonly Color SystemFaceColor = new Color(67);

        /// <summary>System window text colour for chart border lines</summary>
        public static readonly Color SystemWindowTextColorForChartBorderLines = new Color(77);

        /// <summary>System window background colour for chart areas</summary>
        public static readonly Color SystemWindowBackgroundColorForChartAreas = new Color(78);

        /// <summary>Automatic colour for chart border lines (seems to be always Black)</summary>
        public static readonly Color SystemAutomaticColorForChartBorderLines = new Color(79);

        /// <summary>System tool tip background colour (used in note objects)</summary>
        public static readonly Color SystemToolTipBackgroundColor = new Color(80);

        /// <summary>System tool tip text colour (used in note objects)</summary>
        public static readonly Color SystemToolTipTextColor = new Color(81);

        //TODO: Figure out the SystemWindowTextColorForFonts value
        /// <summary>System window text colour for fonts (used in records FONT, EFONT, and CF)</summary>
        //public static readonly Color SystemWindowTextColorForFonts = new Color(??);

        internal static readonly Color DefaultLineColor = Black;
        internal static readonly Color DefaultPatternColor = new Color(64);
        internal static readonly Color DefaultPatternBackgroundColor = SystemWindowBackgroundColorForPatternBackground;
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Describes a range of columns and properties to set on those columns (column width, etc.).
    /// </summary>
    public class ColumnInfo
    {
        private readonly XlsDocument _doc;
        private readonly Worksheet _worksheet;

        private ushort _colIdxStart = 0;
        private ushort _colIDxEnd = 0;
        private ushort _width = 2560; //Set default to 10-character column width
        private bool _hidden;
        private bool _collapsed;
        private byte _outlineLevel;

        /// <summary>
        /// Initializes a new instance of the ColumnInfo class for the given Doc
        /// and Worksheet.
        /// </summary>
        /// <param name="doc">The parent MyXls.Doc object for the new ColumnInfo object.</param>
        /// <param name="worksheet">The parent MyXls.Worksheet object for the new ColumnInfo object.</param>
        public ColumnInfo(XlsDocument doc, Worksheet worksheet)
        {
            _doc = doc;
            _worksheet = worksheet;
        }

        /// <summary>
        /// Gets or sets index to first column in the range.
        /// </summary>
        public ushort ColumnIndexStart
        {
            get { return _colIdxStart; }
            set
            {
                _colIdxStart = value;
                if (_colIDxEnd < _colIdxStart)
                    _colIDxEnd = _colIdxStart;
            }
        }

        /// <summary>
        /// Gets or set index to last column in the range.
        /// </summary>
        public ushort ColumnIndexEnd
        {
            get { return _colIDxEnd; }
            set
            {
                _colIDxEnd = value;
                if (_colIdxStart > _colIDxEnd)
                    _colIdxStart = _colIDxEnd;
            }
        }

        /// <summary>
        /// Gets or sets width of the columns in 1/256 of the width of the zero character, using default font (first FONT record in the file).
        /// </summary>
        public ushort Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Gets or sets XF record (?5.114) for default column formatting
        /// </summary>
        public XF ExtendedFormat
        {
            get { throw new NotSupportedException("ColumnInfo.get_ExtendedFormat"); }
            set { throw new NotSupportedException("ColumnInfo.set_ExtendedFormat"); }
        }

        /// <summary>
        /// Gets or sets whether the columns included in this ColumnInfo definition are hidden.
        /// </summary>
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        /// <summary>
        /// Gets or sets whether the columns included in this ColumnInfo definition are collapsed.
        /// </summary>
        public bool Collapsed
        {
            get { return _collapsed; }
            set { _collapsed = value; }
        }

        /// <summary>
        /// Gets or sets the outline level of the columns (0 = no outline).
        /// </summary>
        public byte OutlineLevel
        {
            get { return _outlineLevel; }
            set
            {
                if (value > 0x07)
                    throw new ArgumentException(string.Format("value {0} must be between 0 and 7", value)); _outlineLevel = value;
            }
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                bytes.Append(BitConverter.GetBytes(_colIdxStart));
                bytes.Append(BitConverter.GetBytes(_colIDxEnd));
                bytes.Append(BitConverter.GetBytes(_width));
                bytes.Append(BitConverter.GetBytes((ushort)0)); //Index to XF record for default column formatting
                bytes.Append(COLINFO_OPTION_FLAGS());
                bytes.Append(new byte[0]); //Not used

                return Record.GetBytes(RID.COLINFO, bytes);
            }
        }

        private Bytes COLINFO_OPTION_FLAGS()
        {
            bool[] bits = new bool[16];
            bits[0] = _hidden;
            Bytes outlineBytes = new Bytes(_outlineLevel);
            bool[] outlineLevelBits = outlineBytes.GetBits().Get(3).Values;
            outlineLevelBits.CopyTo(bits, 8);
            bits[12] = _collapsed;

            return new Bytes.Bits(bits).GetBytes();
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Text cell escapement (sub/super-script, etc.) types.
    /// </summary>
    public enum EscapementTypes : ushort
    {
        /// <summary>Default - None</summary>
        Default = None,

        /// <summary>None</summary>
        None = 0,

        /// <summary>Superscript</summary>
        Superscript = 256,

        /// <summary>Subscript</summary>
        Subscript = 512
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Describes appearance of text in cells.
    /// </summary>
    public class Font : ICloneable
    {
        /*
            ----------------------------------------------------------------------------
            The font with index 4 is omitted in all BIFF versions. This means the first 
            four fonts have zero-based indexes, and the fifth font and all following 
            fonts are referenced with one-based indexes.
                                                - excelfileformat.pdf, section 6.43 FONT
            ----------------------------------------------------------------------------
        */
        private readonly XlsDocument _doc;
        private XF _target;
        private ushort? _id;

        private bool _isInitializing = false;

        private ushort _height;
        private bool _italic;
        private bool _underlined; //NOT IMPLEMENTED - USE 'Underline'
        private bool _struckOut;
        private ushort _colorIndex;
        private FontWeight _weight;
        private EscapementTypes _escapement;
        private UnderlineTypes _underline;
        private FontFamilies _fontFamily;
        private CharacterSets _characterSet;
        private byte _notUsed = 0x00;
        private string _fontName;

        internal Font(XlsDocument doc)
        {
            _isInitializing = true;

            _doc = doc;
            _id = null;

            SetDefaults();

            _isInitializing = false;
        }

        internal Font(XlsDocument doc, XF xf)
            : this(doc)
        {
            _target = xf;
        }

        internal Font(XlsDocument doc, Bytes bytes)
            : this(doc)
        {
            ReadBytes(bytes);
        }

        /// <summary>
        /// Calculates whether a given Font object is value-equal to this
        /// Font object.
        /// </summary>
        /// <param name="that">A Font object to compare to this Font.</param>
        /// <returns>true if equal, false otherwise</returns>
        public bool Equals(Font that)
        {
            if (_height != that._height) return false;
            if (_italic != that._italic) return false;
            if (_underlined != that._underlined) return false;
            if (_struckOut != that._struckOut) return false;
            if (_colorIndex != that._colorIndex) return false;
            if (_weight != that._weight) return false;
            if (_escapement != that._escapement) return false;
            if (_underline != that._underline) return false;
            if (_fontFamily != that._fontFamily) return false;
            if (_characterSet != that._characterSet) return false;
            if (string.Compare(_fontName, that._fontName, false) != 0) return false;

            return true;
        }

        private void SetDefaults()
        {
            _height = 200;
            _italic = false;
            _underlined = false;
            _struckOut = false;
            _colorIndex = 32767;
            _weight = FontWeight.Normal;
            _escapement = EscapementTypes.Default;
            _underline = UnderlineTypes.Default;
            _fontFamily = FontFamilies.Default;
            _characterSet = CharacterSets.Default;
            _notUsed = 0;
            _fontName = "Arial";

            OnChange();
        }

        internal ushort ID
        {
            get
            {
                if (_id == null)
                    _id = _doc.Workbook.Fonts.Add(this);
                return (ushort)_id;
            }
            set
            {
                _id = value;
            }
        }

        private void OnChange()
        {
            if (_isInitializing)
                return;

            _id = null;
            _id = ID;
            _target.OnFontChange(this);
        }

        /// <summary>
        /// Gets or sets the Height of Font (in twips = 1/20 of a point).
        /// </summary>
        public ushort Height
        {
            get { return _height; }
            set
            {
                if (value == _height)
                    return;

                _height = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets whether Characters are italic.
        /// </summary>
        public bool Italic
        {
            get { return _italic; }
            set
            {
                if (value == _italic)
                    return;

                _italic = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets whether Characters are struck out.
        /// </summary>
        public bool StruckOut
        {
            get { return _struckOut; }
            set
            {
                if (value == _struckOut)
                    return;

                _struckOut = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Color index of this Font.
        /// </summary>
        public ushort ColorIndex
        {
            get { return _colorIndex; }
            set
            {
                if (value == _colorIndex)
                    return;

                _colorIndex = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets whether this Font is Bold.
        /// </summary>
        public bool Bold
        {
            get { return (ushort)_weight >= (ushort)FontWeight.Bold; }
            set { Weight = value ? FontWeight.Bold : FontWeight.Normal; }
        }

        ///<summary>Gets or sets Font weight. </summary>
        /// <remarks>This replaces the Bold property.</remarks>
        public FontWeight Weight
        {
            get { return _weight; }
            set
            {
                if (value != _weight)
                {
                    _weight = value;
                    OnChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the EscapementTypes value of this Font.
        /// </summary>
        public EscapementTypes Escapement
        {
            get { return _escapement; }
            set
            {
                if (value == _escapement)
                    return;

                _escapement = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets UnderlineTypes for this Font.  This replaces the 'Underlined' property.
        /// </summary>
        public UnderlineTypes Underline
        {
            get { return _underline; }
            set
            {
                if (value == _underline)
                    return;

                _underline = value;
                OnChange();
            }
        }

        /// <summary>
        /// /Gets or sets Font Family.
        /// </summary>
        public FontFamilies FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (value == _fontFamily)
                    return;

                _fontFamily = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Character Set.
        /// </summary>
        public CharacterSets CharacterSet
        {
            get { return _characterSet; }
            set
            {
                if (value == _characterSet)
                    return;

                _characterSet = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Font Name (255 characters max)
        /// </summary>
        public string FontName
        {
            get { return _fontName; }
            set
            {
                if (value == null)
                    value = string.Empty;

                if (value.Length > 255)
                    value = value.Substring(0, 255);

                if (string.Compare(value, _fontName, true) == 0)
                    return;

                _fontName = value;
                OnChange();
            }
        }

        internal XF Target
        {
            get { return _target; }
            set { _target = value; }
        }

        private void ReadBytes(Bytes bytes)
        {
            byte[] byteArray = bytes.ByteArray;
            _height = BitConverter.ToUInt16(byteArray, 0);
            SetOptionsValue(bytes.Get(2, 2));
            _colorIndex = BitConverter.ToUInt16(byteArray, 4);
            _weight = FontWeightConverter.Convert(BitConverter.ToUInt16(byteArray, 6));
            _escapement = (EscapementTypes)BitConverter.ToUInt16(byteArray, 8);
            _underline = (UnderlineTypes)byteArray[10];
            _fontFamily = (FontFamilies)byteArray[11];
            _characterSet = (CharacterSets)byteArray[12];
            //skip byte index 13
            _fontName = UnicodeBytes.Read(bytes.Get(14, bytes.Length - 14), 8);
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                bytes.Append(BitConverter.GetBytes(_height));
                bytes.Append(BitConverter.GetBytes(OptionsValue()));
                bytes.Append(BitConverter.GetBytes(_colorIndex));
                bytes.Append(BitConverter.GetBytes((ushort)_weight));
                bytes.Append(BitConverter.GetBytes((ushort)_escapement));
                bytes.Append((byte)_underline);
                bytes.Append((byte)_fontFamily);
                bytes.Append((byte)_characterSet);
                bytes.Append(_notUsed);
                bytes.Append(XlsDocument.GetUnicodeString(_fontName, 8));

                return Record.GetBytes(RID.FONT, bytes);
            }
        }

        private void SetOptionsValue(Bytes bytes)
        {
            ushort options = BitConverter.ToUInt16(bytes.ByteArray, 0);

            if (options >= 8)
            {
                _struckOut = true;
                options -= 8;
            }
            else
                _struckOut = false;

            if (options >= 4)
            {
                _underlined = true;
                options -= 4;
            }
            else
                _underlined = false;

            if (options >= 2)
            {
                _italic = true;
            }
            else
                _italic = false;
        }

        private ushort OptionsValue()
        {
            ushort options = 0;

            if (Bold) options += 1;
            if (_italic) options += 2;
            if (_underlined) options += 4;
            if (_struckOut) options += 8;

            return options;
        }

        #region ICloneable members

        /// <summary>
        /// Creates and returns a new Font object value-equal to this Font object.
        /// </summary>
        /// <returns>A new Font object value-equal to this Font object.</returns>
        public object Clone()
        {
            Font clone = new Font(_doc, _target);

            clone._height = _height;
            clone._italic = _italic;
            //clone._underlined = _underlined;
            clone._struckOut = _struckOut;
            clone._colorIndex = _colorIndex;
            clone._weight = _weight;
            clone._escapement = _escapement;
            clone._underline = _underline;
            clone._fontFamily = _fontFamily;
            clone._characterSet = _characterSet;
            clone._fontName = _fontName;

            return clone;
        }

        #endregion
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// The Font Families available for formatting in Excel.
    /// </summary>
    public enum FontFamilies : byte
    {
        /// <summary>Default Font Family : None (unknown or don't care)</summary>
        Default = None,

        /// <summary>None (unknown or don't care)</summary>
        None = 0x00,

        /// <summary>Roman (variable width, serifed)</summary>
        Roman = 0x01,

        /// <summary>Swiss (variable width, sans-serifed)</summary>
        Swiss = 0x02,

        /// <summary>Modern (fixed width, serifed or sans-serifed)</summary>
        Modern = 0x03,

        /// <summary>Script (cursive)</summary>
        Script = 0x04,

        /// <summary>Decorative (specialised, for example Old English, Fraktur)</summary>
        Decorative = 0x05
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// A collection of Font objects.
    /// </summary>
    public class Fonts
    {
        //	----------------------------------------------------------------------------
        //	The font with index 4 is omitted in all BIFF versions. This means the first 
        //	four fonts have zero-based indexes, and the fifth font and all following 
        //	fonts are referenced with one-based indexes.
        //	                                    - excelfileformat.pdf, section 6.43 FONT
        //	----------------------------------------------------------------------------
        private readonly XlsDocument _doc;

        private readonly List<Font> _fonts;

        /// <summary>
        /// Initializes a new instance of the Fonts collection for the given XlsDocument.
        /// </summary>
        /// <param name="doc">The parent XlsDocument object for the new Fonts collection.</param>
        public Fonts(XlsDocument doc)
        {
            _doc = doc;

            _fonts = new List<Font>();

            AddDefaultFonts();
        }

        private void AddDefaultFonts()
        {
            Font font = new Font(_doc, (XF)null);
            _fonts.Add(font);
            _fonts.Add((Font)font.Clone());
            _fonts.Add((Font)font.Clone());
            _fonts.Add((Font)font.Clone());
            _fonts.Add((Font)font.Clone()); //we won't write this one out - just leave it here to fill the 4-index that's never written
        }

        /// <summary>
        /// Adds a new Font object to this collection.
        /// </summary>
        /// <param name="font">The Font object to add to this collection.</param>
        /// <returns>The id of the Font within this collection.</returns>
        public ushort Add(Font font)
        {
            ushort? fontId = GetId(font);

            if (fontId == null)
            {
                fontId = (ushort)_fonts.Count;
                _fonts.Add((Font)font.Clone());
            }

            return (ushort)fontId;
        }

        /// <summary>
        /// Gets the id of the specified Font in this collection.
        /// </summary>
        /// <param name="font">The Font for which to return an id.</param>
        /// <returns>the ushort id of the Font if it exists in this collection, 
        /// null otherwise</returns>
        public ushort? GetId(Font font)
        {
            for (ushort i = 0; i < (ushort)_fonts.Count; i++)
            {
                if (_fonts[i].Equals(font))
                    return i;
            }

            return null;
        }

        internal Font this[ushort idx]
        {
            get { return (Font)_fonts[idx].Clone(); }
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                int i = -1;
                foreach (Font font in _fonts)
                {
                    i++;
                    if (i == 4)
                        continue;
                    bytes.Append(font.Bytes);
                }

                return bytes;
            }
        }
    }
}
namespace Core.MyXls
{
    /// <summary>Relative Weight of a font.</summary>
    /// <remarks>Based on the <a href="http://www.microsoft.com/typography/otspec/otff.htm">OpenType</a> specification.</remarks>
    public enum FontWeight : ushort
    {
        /// <summary>Thin (100) font weight.</summary>
        Thin = 100,

        /// <summary>Extra light (200) font weight.</summary>
        ExtraLight = 200,

        /// <summary>Light (300) font weight.</summary>
        Light = 300,

        /// <summary>Normal (400) font weight.</summary>
        Normal = 400,

        /// <summary>Medium (500) font weight.</summary>
        Medium = 500,

        /// <summary>Semi-Bold (600) font weight.</summary>
        SemiBold = 600,

        /// <summary>Bold (700) font weight.</summary>
        Bold = 700,

        /// <summary>Extra bold (800) font weight.</summary>
        ExtraBold = 800,

        /// <summary>Heavy (900) font weight.</summary>
        Heavy = 900
    }

    /// <summary>Conversions for FontWeight.</summary>
    public static class FontWeightConverter
    {
        /// <summary>Convert from a ushort to a FontWeight value.</summary>
        /// <param name="weight">Font weight value</param>
        /// <returns>FontWeight</returns>
        public static FontWeight Convert(ushort weight)
        {
            FontWeight result = FontWeight.Thin;
            if (weight >= 900)
            {
                result = FontWeight.Heavy;
            }
            else if (weight > 100)
            {
                result = (FontWeight)(Math.Round(weight / 100.0) * 100);
            }
            return result;
        }
    }
}

namespace Core.MyXls
{
    // <summary> Represents a number Format (Currency, Percent, Fraction, Date, Time, etc.).  Currently unimplemented. </summary>
    public class Format : ICloneable
    {
        //<summary>General: General</summary>
        public static readonly string General = "General";

        //<summary>Decimal 1: 0</summary>
        public static readonly string Decimal_1 = "0";

        //<summary>Decimal 2: 0.00</summary>
        public static readonly string Decimal_2 = "0.00";

        //<summary>Decimal 3: #,##0</summary>
        public static readonly string Decimal_3 = "#,##0";

        //<summary>Decimal 4: #,##0.00</summary>
        public static readonly string Decimal_4 = "#,##0.00";

        //<summary>Currency 1: "$"#,##0_);("$"#,##0)</summary>
        public static readonly string Currency_1 = "\"$\"#,##0_);(\"$\"#,##0)";

        //<summary>Currency 2: "$"#,##0_);[Red]("$"#,##0)</summary>
        public static readonly string Currency_2 = "\"$\"#,##0_);[Red](\"$\"#,##0)";

        //<summary>Currency 3: "$"#,##0.00_);("$"#,##0.00)</summary>
        public static readonly string Currency_3 = "\"$\"#,##0.00_);(\"$\"#,##0.00)";

        //<summary>Currency 4: "$"#,##0.00_);[Red]("$"#,##0.00)</summary>
        public static readonly string Currency_4 = "\"$\"#,##0.00_);[Red](\"$\"#,##0.00)";

        //<summary>Percent 1: 0%</summary>
        public static readonly string Percent_1 = "0%";

        //<summary>Percent 2: 0.00%</summary>
        public static readonly string Percent_2 = "0.00%";

        //<summary>Scientific 1: 0.00E+00</summary>
        public static readonly string Scientific_1 = "0.00E+00";

        //<summary>Fraction 1: # ?/?</summary>
        public static readonly string Fraction_1 = "# ?/?";

        //<summary>Fraction 2: # ??/??</summary>
        public static readonly string Fraction_2 = "# ??/??";

        //<summary>Date 1: M/D/YY</summary>
        public static readonly string Date_1 = "M/D/YY";

        //<summary>Date 2: D-MMM-YY</summary>
        public static readonly string Date_2 = "D-MMM-YY";

        //<summary>Date 3: D-MMM</summary>
        public static readonly string Date_3 = "D-MMM";

        //<summary>Date 4: MMM-YY</summary>
        public static readonly string Date_4 = "MMM-YY";

        //<summary>Time 1: h:mm AM/PM</summary>
        public static readonly string Time_1 = "h:mm AM/PM";

        //<summary>Time 2: h:mm:ss AM/PM</summary>
        public static readonly string Time_2 = "h:mm:ss AM/PM";

        //<summary>Time 3: h:mm</summary>
        public static readonly string Time_3 = "h:mm";

        //<summary>Time 4: h:mm:ss</summary>
        public static readonly string Time_4 = "h:mm:ss";

        //<summary>Date/Time: M/D/YY h:mm</summary>
        public static readonly string Date_Time = "M/D/YY h:mm";

        //<summary>Accounting 1: _(#,##0_);(#,##0)</summary>
        public static readonly string Accounting_1 = "_(#,##0_);(#,##0)";

        //<summary>Accounting 2: _(#,##0_);[Red](#,##0)</summary>
        public static readonly string Accounting_2 = "_(#,##0_);[Red](#,##0)";

        //<summary>Accounting 3: _(#,##0.00_);(#,##0.00)</summary>
        public static readonly string Accounting_3 = "_(#,##0.00_);(#,##0.00)";

        //<summary>Accounting 4: _(#,##0.00_);[Red](#,##0.00)</summary>
        public static readonly string Accounting_4 = "_(#,##0.00_);[Red](#,##0.00)";

        //<summary>Currency 5: _("$"* #,##0_);_("$"* (#,##0);_("$"* "-"_);_(@_)</summary>
        public static readonly string Currency_5 = "_(\"$\"* #,##0_);_(\"$\"* (#,##0);_(\"$\"* \"-\"_);_(@_)";

        //<summary>Currency 6: _(* #,##0_);_(* (#,##0);_(* "-"_);_(@_)</summary>
        public static readonly string Currency_6 = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";

        //<summary>Currency 7: _("$"* #,##0.00_);_("$"* (#,##0.00);_("$"* "-"??_);_(@_)</summary>
        public static readonly string Currency_7 = "_(\"$\"* #,##0.00_);_(\"$\"* (#,##0.00);_(\"$\"* \"-\"??_);_(@_)";

        //<summary>Currency 8: _(* #,##0.00_);_(* (#,##0.00);_(* "-"??_);_(@_)</summary>
        public static readonly string Currency_8 = "_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)";

        //<summary>Time 5: mm:ss</summary>
        public static readonly string Time_5 = "mm:ss";

        //<summary>Time 6: [h]:mm:ss</summary>
        public static readonly string Time_6 = "[h]:mm:ss";

        //<summary>Time 7: mm:ss.0</summary>
        public static readonly string Time_7 = "mm:ss.0";

        //<summary>Scientific 2: ##0.0E+0</summary>
        public static readonly string Scientific_2 = "##0.0E+0";

        //<summary>Text: @</summary>
        public static readonly string Text = "@";

        private XlsDocument _doc;
        private XF _xf;

        private bool _isInitializing = false;

        private ushort? _id = null;
        private string _formatString = string.Empty;

        internal Format(XlsDocument doc, XF xf)
        {
            _isInitializing = true;

            _doc = doc;
            _xf = xf;

            _isInitializing = false;
        }

        internal Format(XlsDocument doc, Bytes bytes)
            : this(doc, (XF)null)
        {
            _isInitializing = true;

            ReadBytes(bytes);

            _isInitializing = false;
        }

        private void OnChange()
        {
            if (_isInitializing)
                return;

            _id = null;
            _xf.OnChange();
        }

        internal XF ParentXf
        {
            get { return _xf; }
            set { _xf = value; }
        }

        internal ushort ID
        {
            get
            {
                if (_id == null)
                    _id = _doc.Workbook.Formats.Add(this.String);
                return (ushort)_id;
            }
        }

        //<summary>
        //Gets or sets this Format's String.
        //</summary>
        public string String
        {
            get { return _formatString; }
            set
            {
                if (value == null)
                    value = string.Empty;

                if (value.Length > 65535)
                    value = value.Substring(0, 65535);

                if (string.Compare(value, _formatString, false) == 0)
                    return;

                _formatString = value;
                OnChange();
            }
        }

        //<summary>
        //Returns whether the given Format object is value-equal to this Format object.
        //</summary>
        //<param name="that">A Format object to compare to this Format object.</param>
        //<returns>true if the provided Format object is value-equal to this Format object,
        //false otherwise</returns>
        public bool Equals(Format that)
        {
            if (string.Compare(_formatString, that._formatString, false) != 0)
                return false;

            return true;
        }

        private void ReadBytes(Bytes bytes)
        {
            //_formatString = XlsDocument.ReadBinUniStr2(bytes.Get(2, bytes.Length - 2));
            //TODO WH 
            _formatString = System.Text.Encoding.Unicode.GetString(bytes.Get(2, bytes.Length - 2).ByteArray);
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                bytes.Append(BitConverter.GetBytes(ID));
                //TODO WH 
                bytes.Append(System.Text.Encoding.Unicode.GetBytes(String));
                //bytes.Append(  XlsDocument.BinUniStr2(String));

                return Record.GetBytes(RID.FORMAT, bytes);
            }
        }

        #region ICloneable members

        //<summary> 
        //Returns a new Format object which is value-equal to this Format object.
        // </summary>
        // <returns>A new Format object value-equal to this Format object.</returns>
        public object Clone()
        {
            Format clone = new Format(this._doc, this._xf);

            clone._formatString = this._formatString;

            return clone;
        }

        #endregion
    }
}

namespace Core.MyXls
{
    /// <summary>
    /// A collection class to manage Formats for an XlsDocument.
    /// </summary>
    public class Formats
    {
        private readonly XlsDocument _doc;

        private static readonly Dictionary<string, ushort> _defaultFormatIds = new Dictionary<string, ushort>();
        private readonly Dictionary<string, ushort> _userFormatIds = new Dictionary<string, ushort>();
        private static List<string> _defaultFormatsToWrite = new List<string>();

        internal static string Default = StandardFormats.General;

        private ushort _nextUserFormatId = 164;

        static Formats()
        {
            AddDefaults();

            _defaultFormatsToWrite.Add(StandardFormats.Currency_1);
        }

        internal Formats(XlsDocument doc)
        {
            _doc = doc;
        }

        private static void AddDefaults()
        {
            _defaultFormatIds[StandardFormats.General] = 0;
            _defaultFormatIds[StandardFormats.Decimal_1] = 1;
            _defaultFormatIds[StandardFormats.Decimal_2] = 2;
            _defaultFormatIds[StandardFormats.Decimal_3] = 3;
            _defaultFormatIds[StandardFormats.Decimal_4] = 4;
            _defaultFormatIds[StandardFormats.Currency_1] = 5;
            _defaultFormatIds[StandardFormats.Currency_2] = 6;
            _defaultFormatIds[StandardFormats.Currency_3] = 7;
            _defaultFormatIds[StandardFormats.Currency_4] = 8;
            _defaultFormatIds[StandardFormats.Percent_1] = 9;
            _defaultFormatIds[StandardFormats.Percent_2] = 10;
            _defaultFormatIds[StandardFormats.Scientific_1] = 11;
            _defaultFormatIds[StandardFormats.Fraction_1] = 12;
            _defaultFormatIds[StandardFormats.Fraction_2] = 13;
            _defaultFormatIds[StandardFormats.Date_1] = 14;
            _defaultFormatIds[StandardFormats.Date_2] = 15;
            _defaultFormatIds[StandardFormats.Date_3] = 16;
            _defaultFormatIds[StandardFormats.Date_4] = 17;
            _defaultFormatIds[StandardFormats.Time_1] = 18;
            _defaultFormatIds[StandardFormats.Time_2] = 19;
            _defaultFormatIds[StandardFormats.Time_3] = 20;
            _defaultFormatIds[StandardFormats.Time_4] = 21;
            _defaultFormatIds[StandardFormats.Date_Time] = 22;
            _defaultFormatIds[StandardFormats.Accounting_1] = 37;
            _defaultFormatIds[StandardFormats.Accounting_2] = 38;
            _defaultFormatIds[StandardFormats.Accounting_3] = 39;
            _defaultFormatIds[StandardFormats.Accounting_4] = 40;
            _defaultFormatIds[StandardFormats.Currency_5] = 41;
            _defaultFormatIds[StandardFormats.Currency_6] = 42;
            _defaultFormatIds[StandardFormats.Currency_7] = 43;
            _defaultFormatIds[StandardFormats.Currency_8] = 44;
            _defaultFormatIds[StandardFormats.Time_5] = 45;
            _defaultFormatIds[StandardFormats.Time_6] = 46;
            _defaultFormatIds[StandardFormats.Time_7] = 47;
            _defaultFormatIds[StandardFormats.Scientific_2] = 48;
            _defaultFormatIds[StandardFormats.Text] = 49;
            _defaultFormatIds[StandardFormats.Date_5] = 57;
            _defaultFormatIds[StandardFormats.Date_6] = 58;
        }

        internal string this[ushort index]
        {
            get
            {
                foreach (string format in _userFormatIds.Keys)
                    if (_userFormatIds[format] == index)
                        return format;

                foreach (string format in _defaultFormatIds.Keys)
                    if (_defaultFormatIds[format] == index)
                        return format;

                throw new IndexOutOfRangeException(string.Format("index {0} not found", index));
            }
        }

        /// <summary>
        /// Adds a new Format object to this collection and returns its id.
        /// </summary>
        /// <param name="format">The Format sting to add to this collection.</param>
        /// <returns>The id of the added Format string.</returns>
        public ushort Add(string format)
        {
            return Add(format, null);
        }

        private ushort Add(string format, ushort? id)
        {
            bool isUserFormat = (id == null);

            ushort? existingId = GetID(format);

            bool exists = (existingId != null);

            if (exists)
                return (ushort)existingId;

            if (isUserFormat)
                id = _nextUserFormatId++;

            _userFormatIds[format] = (ushort)id;

            return (ushort)id;
        }

        internal ushort GetFinalID(string format)
        {
            ushort? id = GetID(format);
            if (id == null)
                throw new ApplicationException(string.Format("Format {0} does not exist", format));
            return (ushort)id;
        }

        internal ushort? GetID(string format)
        {
            ushort? id = null;

            if (_defaultFormatIds.ContainsKey(format))
                id = _defaultFormatIds[format];

            if (_userFormatIds.ContainsKey(format))
                id = _userFormatIds[format];

            return id;
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                foreach (string format in _defaultFormatsToWrite)
                {
                    if (_defaultFormatIds.ContainsKey(format))
                        bytes.Append(GetFormatRecord(_defaultFormatIds[format], format));
                }

                foreach (string format in _userFormatIds.Keys)
                    bytes.Append(GetFormatRecord(_userFormatIds[format], format));

                return bytes;
            }
        }

        /// <summary>
        /// Gets the number of Formats currently contained in this Formats collection.
        /// </summary>
        public object Count
        {
            get { return _userFormatIds.Count + _defaultFormatsToWrite.Count; }
        }

        private Bytes GetFormatRecord(ushort id, string format)
        {
            Bytes bytes = new Bytes();

            bytes.Append(BitConverter.GetBytes(id));
            bytes.Append(XlsDocument.GetUnicodeString(format, 16));

            return Record.GetBytes(RID.FORMAT, bytes);
        }

        /// <summary>
        /// Determines whether this Formats collection contains a value with the given index.
        /// </summary>
        /// <param name="index">The index at which to check for a value.</param>
        /// <returns>true if a value exists at the specified index, false otherwise</returns>
        public bool ContainsKey(ushort index)
        {
            return _defaultFormatIds.ContainsValue(index) || _userFormatIds.ContainsValue(index);
        }
    }
}
namespace Core.MyXls
{
    internal class FormulaRecord : Record
    {
        internal Record StringRecord = null;

        internal FormulaRecord(Record formulaRecord, Record stringRecord)
            : base()
        {
            _rid = formulaRecord.RID;
            _data = formulaRecord.Data;
            _continues = formulaRecord.Continues;

            StringRecord = stringRecord;
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// The different horizontal alignments available in Excel.
    /// </summary>
    public enum HorizontalAlignments : byte
    {
        /// <summary>Default - General</summary>
        Default = General,

        /// <summary>General</summary>
        General = 0,

        /// <summary>Left</summary>
        Left = 1,

        /// <summary>Centered</summary>
        Centered = 2,

        /// <summary>Right</summary>
        Right = 3,

        /// <summary>Filled</summary>
        Filled = 4,

        /// <summary>Justified</summary>
        Justified = 5,

        /// <summary>Centered Across the Selection</summary>
        CenteredAcrossSelection = 6,

        /// <summary>Distributed</summary>
        Distributed = 7
    }
}
namespace Core.MyXls
{
    internal interface IXFTarget
    {
        void UpdateId(XF fromXF);
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Line Styles
    /// </summary>
    public enum LineStyle : ushort
    {
        /// <summary>No Style</summary>
        None = 0,

        /// <summary>Thin</summary>
        Thin,

        /// <summary>Medium</summary>
        Medium,

        /// <summary>Dashed</summary>
        Dashed,

        /// <summary>Dotted</summary>
        Dotted,

        /// <summary>Thick</summary>
        Thick,

        /// <summary>Double</summary>
        Double,

        /// <summary>Hair</summary>
        Hair,

        /// <summary>Medium dashed</summary>
        /// <remarks>BIFF8 Only</remarks>
        MediumDashed,

        /// <summary>Dash-dot</summary>
        /// <remarks>BIFF8 Only</remarks>
        DashDot,

        /// <summary>Medium dash-dot</summary>
        /// <remarks>BIFF8 Only</remarks>
        MediumDashDot,

        /// <summary>Dash-dot-dot</summary>
        /// <remarks>BIFF8 Only</remarks>
        DashDotDot,

        /// <summary>Medium dash-dot-dot</summary>
        /// <remarks>BIFF8 Only</remarks>
        MediumDashDotDot,

        /// <summary>Slanted dash-dot</summary>
        /// <remarks>BIFF8 Only</remarks>
        SlantedDashDot
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Defines a contiguous group of Merged Cells on a Worksheet.
    /// </summary>
    public struct MergeArea
    {
        /// <summary>
        /// The first Row in this group of Merged Cells (1-based).
        /// </summary>
        public ushort RowMin;

        /// <summary>
        /// The last Row in this group of Merged Cells (1-based).
        /// </summary>
        public ushort RowMax;

        /// <summary>
        /// The first Column in this group of Merged Cells (1-based).
        /// </summary>
        public ushort ColMin;

        /// <summary>
        /// The last Column in this group of Merged Cells (1-based).
        /// </summary>
        public ushort ColMax;

        /// <summary>
        /// Initializes a new MergeArea with the provided values.
        /// </summary>
        /// <param name="rowMin">The first Row in this group of Merged Cells (1-based).</param>
        /// <param name="rowMax">The last Row in this group of Merged Cells (1-based).</param>
        /// <param name="colMin">The first Column in this group of Merged Cells (1-based).</param>
        /// <param name="colMax">The last Column in this group of Merged Cells (1-based).</param>
        public MergeArea(ushort rowMin, ushort rowMax, ushort colMin, ushort colMax)
        {
            RowMin = rowMin;
            RowMax = rowMax;
            ColMin = colMin;
            ColMax = colMax;
        }

        /// <summary>
        /// Initializes a new MergeArea with the provided values.
        /// </summary>
        /// <param name="rowMin">The first Row in this group of Merged Cells (1-based).</param>
        /// <param name="rowMax">The last Row in this group of Merged Cells (1-based).</param>
        /// <param name="colMin">The first Column in this group of Merged Cells (1-based).</param>
        /// <param name="colMax">The last Column in this group of Merged Cells (1-based).</param>
        public MergeArea(int rowMin, int rowMax, int colMin, int colMax)
            : this((ushort)rowMin, (ushort)rowMax, (ushort)colMin, (ushort)colMax)
        {
            if (rowMin < 1) throw new ArgumentOutOfRangeException("rowMin", "must be >= 1");
            if (rowMin > BIFF8.MaxRows) throw new ArgumentOutOfRangeException("rowMin", "must be <= " + BIFF8.MaxRows);
            if (rowMax < rowMin) throw new ArgumentOutOfRangeException("rowMax", "must be >= rowMin (" + rowMin + ")");
            if (rowMax > BIFF8.MaxRows) throw new ArgumentOutOfRangeException("rowMax", "must be <=" + BIFF8.MaxRows);
            if (colMin < 1) throw new ArgumentOutOfRangeException("colMin", "must be >= 1");
            if (colMin > BIFF8.MaxCols) throw new ArgumentOutOfRangeException("colMin", "must be <= " + BIFF8.MaxCols);
            if (colMax < colMin) throw new ArgumentOutOfRangeException("colMax", "must be >= colMin (" + colMin + ")");
            if (colMax > BIFF8.MaxCols) throw new ArgumentOutOfRangeException("colMax", "must be <= " + BIFF8.MaxCols);

            //I know these will only be checked after the chained constructor, but 
            Util.ValidateUShort(rowMin, "rowMin");
            Util.ValidateUShort(rowMax, "rowMax");
            Util.ValidateUShort(colMin, "colMin");
            Util.ValidateUShort(colMax, "colMax");
        }
    }
}
namespace Core.MyXls
{
    //http://www.mvps.org/dmcritchie/excel/colors.htm
    internal class Palette
    {
        private Workbook _workbook;
        private List<Color> _egaColors = new List<Color>();
        private List<Color> _colors = new List<Color>();

        internal Palette(Workbook workbook)
        {
            _workbook = workbook;

            InitDefaultPalette();
        }

        private void InitDefaultPalette()
        {
            _egaColors.Add(Colors.EgaBlack);
            _egaColors.Add(Colors.EgaWhite);
            _egaColors.Add(Colors.EgaRed);
            _egaColors.Add(Colors.EgaGreen);
            _egaColors.Add(Colors.EgaBlue);
            _egaColors.Add(Colors.EgaYellow);
            _egaColors.Add(Colors.EgaMagenta);
            _egaColors.Add(Colors.EgaCyan);
            _colors.Add(Colors.Black);
            _colors.Add(Colors.White);
            _colors.Add(Colors.Red);
            _colors.Add(Colors.Green);
            _colors.Add(Colors.Blue);
            _colors.Add(Colors.Yellow);
            _colors.Add(Colors.Magenta);
            _colors.Add(Colors.Cyan);
            _colors.Add(Colors.DarkRed);
            _colors.Add(Colors.DarkGreen);
            _colors.Add(Colors.DarkBlue);
            _colors.Add(Colors.Olive);
            _colors.Add(Colors.Purple);
            _colors.Add(Colors.Teal);
            _colors.Add(Colors.Silver);
            _colors.Add(Colors.Grey);
            _colors.Add(Colors.Default18);
            _colors.Add(Colors.Default19);
            _colors.Add(Colors.Default1A);
            _colors.Add(Colors.Default1B);
            _colors.Add(Colors.Default1C);
            _colors.Add(Colors.Default1D);
            _colors.Add(Colors.Default1E);
            _colors.Add(Colors.Default1F);
            _colors.Add(Colors.Default20);
            _colors.Add(Colors.Default21);
            _colors.Add(Colors.Default22);
            _colors.Add(Colors.Default23);
            _colors.Add(Colors.Default24);
            _colors.Add(Colors.Default25);
            _colors.Add(Colors.Default26);
            _colors.Add(Colors.Default27);
            _colors.Add(Colors.Default28);
            _colors.Add(Colors.Default29);
            _colors.Add(Colors.Default2A);
            _colors.Add(Colors.Default2B);
            _colors.Add(Colors.Default2C);
            _colors.Add(Colors.Default2D);
            _colors.Add(Colors.Default2E);
            _colors.Add(Colors.Default2F);
            _colors.Add(Colors.Default30);
            _colors.Add(Colors.Default31);
            _colors.Add(Colors.Default32);
            _colors.Add(Colors.Default33);
            _colors.Add(Colors.Default34);
            _colors.Add(Colors.Default35);
            _colors.Add(Colors.Default36);
            _colors.Add(Colors.Default37);
            _colors.Add(Colors.Default38);
            _colors.Add(Colors.Default39);
            _colors.Add(Colors.Default3A);
            _colors.Add(Colors.Default3B);
            _colors.Add(Colors.Default3C);
            _colors.Add(Colors.Default3D);
            _colors.Add(Colors.Default3E);
            _colors.Add(Colors.Default3F);
        }

        internal ushort GetIndex(Color color)
        {
            if (_colors.Contains(color))
                return (ushort)(_colors.IndexOf(color) + _egaColors.Count);
            else if (_egaColors.Contains(color))
                return (ushort)_egaColors.IndexOf(color);
            else if (color.Id == null)
                throw new ArgumentOutOfRangeException("Could not locate color in palette");
            else //is system color (not in user palette)
            {
                return (ushort)color.Id;
            }
        }
    }
}
namespace Core.MyXls
{
    internal class Record
    {
        protected byte[] _rid;
        protected Bytes _data = new Bytes();
        protected List<Record> _continues = new List<Record>();

        public static Record Empty = new Record(MyXls.RID.Empty, new byte[0]);

        protected Record()
        {
            //would rather this be a struct, but it's more convenient to be able
            //to use reference equality testing below
        }

        internal Record(byte[] rid, byte[] data)
            : this(rid, new Bytes(data))
        { }

        internal Record(byte[] rid, Bytes data)
        {
            _rid = Core.MyXls.RID.ByteArray(rid);
            int offset = 0;
            int bytesRemaining = data.Length;
            int continueIndex = -1;
            while (bytesRemaining > 0)
            {
                int bytesToAppend = Math.Min(bytesRemaining, BIFF8.MaxDataBytesPerRecord);
                if (continueIndex == -1)
                    _data = data.Get(offset, bytesToAppend);
                else
                    _continues.Add(new Record(MyXls.RID.CONTINUE, data.Get(offset, bytesToAppend)));
                offset += bytesToAppend;
                bytesRemaining -= bytesToAppend;
                continueIndex++;
            }
        }

        internal byte[] RID
        {
            get { return _rid; }
        }

        internal Bytes Data
        {
            get { return _data; }
        }

        internal List<Record> Continues
        {
            get { return _continues; }
        }

        internal static Bytes GetBytes(byte[] rid, byte[] data)
        {
            return GetBytes(rid, new Bytes(data));
        }

        internal static Bytes GetBytes(byte[] rid, Bytes data)
        {
            if (rid.Length != 2)
                throw new ArgumentException("must be 2 bytes", "rid");

            Bytes record = new Bytes();

            ushort offset = 0;
            ushort totalLength = (ushort)data.Length;
            do
            {
                ushort length = Math.Min((ushort)(totalLength - offset), BIFF8.MaxDataBytesPerRecord);

                if (offset == 0)
                {
                    record.Append(rid);
                    record.Append(BitConverter.GetBytes(length));
                    record.Append(data.Get(offset, length));
                }
                else
                {
                    record.Append(MyXls.RID.CONTINUE);
                    record.Append(BitConverter.GetBytes(length));
                    record.Append(data.Get(offset, length));
                }

                offset += length;
            } while (offset < totalLength);

            return record;
        }

        internal bool IsCellRecord()
        {
            return (_rid == MyXls.RID.RK ||
                    _rid == MyXls.RID.NUMBER ||
                    _rid == MyXls.RID.LABEL ||
                    _rid == MyXls.RID.LABELSST ||
                    _rid == MyXls.RID.MULBLANK ||
                    _rid == MyXls.RID.MULRK ||
                    _rid == MyXls.RID.FORMULA);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static List<Record> GetAll(Bytes stream)
        {
            int i = 0;
            List<Record> records = new List<Record>();
            Record lastNonContinue = Record.Empty;
            while (i < (stream.Length - 4))
            {
                byte[] rid = Core.MyXls.RID.ByteArray(stream.Get(i, 2).ByteArray);
                Bytes data = new Bytes();
                if (rid == MyXls.RID.Empty)
                    break;
                int length = BitConverter.ToUInt16(stream.Get(i + 2, 2).ByteArray, 0);
                data = stream.Get(i + 4, length);
                Record record = new Record(rid, data);
                i += (4 + length);
                if (rid == MyXls.RID.CONTINUE)
                {
                    if (lastNonContinue == Record.Empty)
                        throw new ApplicationException("Found CONTINUE record without previous/parent record.");

                    lastNonContinue.Continues.Add(record);
                }
                else
                {
                    lastNonContinue = record;
                    records.Add(record);
                }
            }

            return records;
        }
    }
}
namespace Core.MyXls
{
    internal static class RID
    {
        public static readonly byte[] Empty = new byte[2] { 0x00, 0x00 };

        internal static readonly byte[] ARRAY = new byte[2] { 0x21, 0x02 };
        internal static readonly byte[] BACKUP = new byte[2] { 0x40, 0x00 };
        internal static readonly byte[] BITMAP = new byte[2] { 0xE9, 0x00 };
        internal static readonly byte[] BLANK = new byte[2] { 0x01, 0x02 };
        internal static readonly byte[] BOF = new byte[2] { 0x09, 0x08 };
        internal static readonly byte[] BOOKBOOL = new byte[2] { 0xDA, 0x00 };
        internal static readonly byte[] BOOLERR = new byte[2] { 0x05, 0x02 };
        internal static readonly byte[] BOTTOMMARGIN = new byte[2] { 0x29, 0x00 };
        internal static readonly byte[] BOUNDSHEET = new byte[2] { 0x85, 0x00 };
        internal static readonly byte[] CALCCOUNT = new byte[2] { 0x0C, 0x00 };
        internal static readonly byte[] CALCMODE = new byte[2] { 0x0D, 0x00 };
        internal static readonly byte[] CODEPAGE = new byte[2] { 0x42, 0x00 };
        internal static readonly byte[] COLINFO = new byte[2] { 0x7D, 0x00 };
        internal static readonly byte[] CONDFMT = new byte[2] { 0xB0, 0x01 };
        internal static readonly byte[] CONTINUE = new byte[2] { 0x3C, 0x00 };
        internal static readonly byte[] COUNTRY = new byte[2] { 0x8C, 0x00 };
        internal static readonly byte[] CRN = new byte[2] { 0x5A, 0x00 };
        internal static readonly byte[] DATEMODE = new byte[2] { 0x22, 0x00 };
        internal static readonly byte[] DBCELL = new byte[2] { 0xD7, 0x00 };
        internal static readonly byte[] DCONREF = new byte[2] { 0x51, 0x00 };
        internal static readonly byte[] DEFAULTROWHEIGHT = new byte[2] { 0x25, 0x02 };
        internal static readonly byte[] DEFCOLWIDTH = new byte[2] { 0x55, 0x00 };
        internal static readonly byte[] DELTA = new byte[2] { 0x10, 0x00 };
        internal static readonly byte[] DIMENSIONS = new byte[2] { 0x00, 0x02 };
        internal static readonly byte[] DSF = new byte[2] { 0x61, 0x01 };
        internal static readonly byte[] DV = new byte[2] { 0xBE, 0x01 };
        internal static readonly byte[] DVAL = new byte[2] { 0xB2, 0x01 };
        internal static readonly byte[] EOF = new byte[2] { 0x0A, 0x00 };
        internal static readonly byte[] EXTERNNAME = new byte[2] { 0x23, 0x00 };
        internal static readonly byte[] EXTERNSHEET = new byte[2] { 0x17, 0x00 };
        internal static readonly byte[] EXTSST = new byte[2] { 0xFF, 0x00 };
        internal static readonly byte[] FILEPASS = new byte[2] { 0x2F, 0x00 };
        internal static readonly byte[] FILESHARING = new byte[2] { 0x5B, 0x00 };
        internal static readonly byte[] FONT = new byte[2] { 0x31, 0x00 };
        internal static readonly byte[] FOOTER = new byte[2] { 0x15, 0x00 };
        internal static readonly byte[] FORMAT = new byte[2] { 0x1E, 0x04 };
        internal static readonly byte[] FORMULA = new byte[2] { 0x06, 0x00 };
        internal static readonly byte[] GRIDSET = new byte[2] { 0x82, 0x00 };
        internal static readonly byte[] GUTS = new byte[2] { 0x80, 0x00 };
        internal static readonly byte[] HCENTER = new byte[2] { 0x83, 0x00 };
        internal static readonly byte[] HEADER = new byte[2] { 0x14, 0x00 };
        internal static readonly byte[] HIDEOBJ = new byte[2] { 0x8D, 0x00 };
        internal static readonly byte[] HLINK = new byte[2] { 0xB8, 0x01 };
        internal static readonly byte[] HORIZONTALPAGEBREAKS = new byte[2] { 0x1B, 0x00 };
        internal static readonly byte[] INDEX = new byte[2] { 0x0B, 0x02 };
        internal static readonly byte[] ITERATION = new byte[2] { 0x11, 0x00 };
        internal static readonly byte[] LABEL = new byte[2] { 0x04, 0x02 };
        internal static readonly byte[] LABELRANGES = new byte[2] { 0x5F, 0x01 };
        internal static readonly byte[] LABELSST = new byte[2] { 0xFD, 0x00 };
        internal static readonly byte[] LEFTMARGIN = new byte[2] { 0x26, 0x00 };
        internal static readonly byte[] MERGEDCELLS = new byte[2] { 0xE5, 0x00 };
        internal static readonly byte[] MULBLANK = new byte[2] { 0xBE, 0x00 };
        internal static readonly byte[] MULRK = new byte[2] { 0xBD, 0x00 };
        internal static readonly byte[] NAME = new byte[2] { 0x18, 0x00 };
        internal static readonly byte[] NOTE = new byte[2] { 0x1C, 0x00 };
        internal static readonly byte[] NUMBER = new byte[2] { 0x03, 0x02 };
        internal static readonly byte[] OBJECTPROTECT = new byte[2] { 0x63, 0x00 };
        internal static readonly byte[] PALETTE = new byte[2] { 0x92, 0x00 };
        internal static readonly byte[] PANE = new byte[2] { 0x41, 0x00 };
        internal static readonly byte[] PASSWORD = new byte[2] { 0x13, 0x00 };
        internal static readonly byte[] PHONETIC = new byte[2] { 0xEF, 0x00 };
        internal static readonly byte[] PRECISION = new byte[2] { 0x0E, 0x00 };
        internal static readonly byte[] PRINTGRIDLINES = new byte[2] { 0x2B, 0x00 };
        internal static readonly byte[] PRINTHEADERS = new byte[2] { 0x2A, 0x00 };
        internal static readonly byte[] PROTECT = new byte[2] { 0x12, 0x00 };
        internal static readonly byte[] QUICKTIP = new byte[2] { 0x00, 0x08 };
        internal static readonly byte[] RANGEPROTECTION = new byte[2] { 0x68, 0x08 };
        internal static readonly byte[] REFMODE = new byte[2] { 0x0F, 0x00 };
        internal static readonly byte[] RIGHTMARGIN = new byte[2] { 0x27, 0x00 };
        internal static readonly byte[] RK = new byte[2] { 0x7E, 0x02 };
        internal static readonly byte[] RSTRING = new byte[2] { 0xD6, 0x00 };
        internal static readonly byte[] ROW = new byte[2] { 0x08, 0x02 };
        internal static readonly byte[] SAVERECALC = new byte[2] { 0x5F, 0x00 };
        internal static readonly byte[] SCENPROTECT = new byte[2] { 0xDD, 0x00 };
        internal static readonly byte[] SCL = new byte[2] { 0xA0, 0x00 };
        internal static readonly byte[] SELECTION = new byte[2] { 0x1D, 0x00 };
        internal static readonly byte[] SETUP = new byte[2] { 0xA1, 0x00 };
        internal static readonly byte[] SHEETLAYOUT = new byte[2] { 0x62, 0x08 };
        internal static readonly byte[] SHEETPROTECTION = new byte[2] { 0x67, 0x08 };
        internal static readonly byte[] SHRFMLA = new byte[2] { 0xBC, 0x04 };
        internal static readonly byte[] SORT = new byte[2] { 0x90, 0x00 };
        internal static readonly byte[] SST = new byte[2] { 0xFC, 0x00 };
        internal static readonly byte[] STANDARDWIDTH = new byte[2] { 0x99, 0x00 };
        internal static readonly byte[] STRING = new byte[2] { 0x07, 0x02 };
        internal static readonly byte[] STYLE = new byte[2] { 0x93, 0x02 };
        internal static readonly byte[] SUPBOOK = new byte[2] { 0xAE, 0x01 };
        internal static readonly byte[] TABLEOP = new byte[2] { 0x36, 0x02 };
        internal static readonly byte[] TOPMARGIN = new byte[2] { 0x28, 0x00 };
        internal static readonly byte[] UNCALCED = new byte[2] { 0x5E, 0x00 };
        internal static readonly byte[] USESELFS = new byte[2] { 0x60, 0x01 };
        internal static readonly byte[] VCENTER = new byte[2] { 0x84, 0x00 };
        internal static readonly byte[] VERTICALPAGEBREAKS = new byte[2] { 0x1A, 0x00 };
        internal static readonly byte[] WINDOW1 = new byte[2] { 0x3D, 0x00 };
        internal static readonly byte[] WINDOW2 = new byte[2] { 0x3E, 0x02 };
        internal static readonly byte[] WINDOWPROTECT = new byte[2] { 0x19, 0x00 };
        internal static readonly byte[] WRITEACCESS = new byte[2] { 0x5C, 0x00 };
        internal static readonly byte[] WRITEPROT = new byte[2] { 0x86, 0x00 };
        internal static readonly byte[] WSBOOL = new byte[2] { 0x81, 0x00 };
        internal static readonly byte[] XCT = new byte[2] { 0x59, 0x00 };
        internal static readonly byte[] XF = new byte[2] { 0xE0, 0x00 };

        private static readonly Dictionary<byte, Dictionary<byte, string>> _names = new Dictionary<byte, Dictionary<byte, string>>();
        private static readonly Dictionary<string, byte[]> _rids = new Dictionary<string, byte[]>();
        internal static readonly int NAME_MAX_LENGTH = 0;

        static RID()
        {
            foreach (FieldInfo fi in typeof(RID).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (fi.FieldType == typeof(byte[]))
                {
                    byte[] rid = (byte[])fi.GetValue(null);

                    if (rid.Length == 2)
                    {
                        byte first = rid[0];
                        if (!_names.ContainsKey(first))
                            _names[first] = new Dictionary<byte, string>();
                        _names[first][rid[1]] = fi.Name;
                        _rids[fi.Name] = (byte[])fi.GetValue(null);
                        NAME_MAX_LENGTH = Math.Max(NAME_MAX_LENGTH, fi.Name.Length);
                    }
                }
            }
        }

        internal static string Name(byte[] rid)
        {
            if (_names.ContainsKey(rid[0]) && _names[rid[0]].ContainsKey(rid[1]))
                return _names[rid[0]][rid[1]];
            else
                return string.Format("??? {0:x2} {1:x2}", rid[0], rid[1]);
        }

        internal static byte[] ByteArray(byte[] rid)
        {
            string name = Name(rid);
            if (_rids.ContainsKey(name))
                return _rids[name];
            else
                return rid;
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Represents a single row in a Worksheet.
    /// </summary>
    public class Row
    {
        private readonly SortedList<ushort, Cell> _cells = new SortedList<ushort, Cell>();

        private ushort _rowIndex;
        //		private ushort _cellCount;
        private ushort _minCellCol;
        private ushort _maxCellCol;

        /// <summary>
        /// Initializes a new instance of the Row class.
        /// </summary>
        public Row()
        {
            _minCellCol = 0;
            _maxCellCol = 0;
        }

        /// <summary>
        /// Gets the row index of this Row object.
        /// </summary>
        public ushort RowIndex
        {
            get { return _rowIndex; }
            internal set { _rowIndex = value; }
        }

        /// <summary>
        /// Returns whether a Cell exists on this Row at the specified Column.  A
        /// Cell will exist if a value or property has been specified for it.
        /// </summary>
        /// <param name="atCol">Column at which to check for a Cell in this Row.</param>
        /// <returns>true if a Cell exists on this Row at the specified Column, false otherwise</returns>
        public bool CellExists(ushort atCol)
        {
            return _cells.ContainsKey(atCol);
        }

        /// <summary>
        /// Adds a Cell to this Row.
        /// </summary>
        /// <param name="cell">The Cell to add to this Row</param>
        public void AddCell(Cell cell)
        {
            ushort cCol = cell.Column;

            if (CellExists(cCol))
                throw new Exception(string.Format("Cell already exists at column {0}", cCol));
            if (cCol < 1 || cCol > 256)
                throw new ArgumentOutOfRangeException(string.Format("cell.Col {0} must be between 1 and 256", cCol));

            if (_minCellCol == 0)
            {
                _minCellCol = cCol;
                _maxCellCol = cCol;
            }
            else
            {
                if (cCol < _minCellCol)
                    _minCellCol = cCol;
                else if (cCol > _maxCellCol)
                    _maxCellCol = cCol;
            }

            _cells.Add(cCol, cell);
        }

        /// <summary>
        /// Gets the count of Cells that exists on this Row.
        /// </summary>
        public ushort CellCount
        {
            get { return (ushort)_cells.Count; }
        }

        /// <summary>
        /// Returns the Cell at the specified column on this Row.
        /// </summary>
        /// <param name="col">The column from which to return the Cell.</param>
        /// <returns>The Cell from the specified column on this Row.</returns>
        public Cell CellAtCol(ushort col)
        {
            if (!CellExists(col))
                throw new Exception(string.Format("Cell at col {0} does not exist", col));

            return _cells[col];
        }

        /// <summary>
        /// Returns the Cell with the specified index from the existing Cells on this Row.
        /// (i.e. if there are three cells at columns 1, 3, and 5, specifying 2 will return
        /// the Cell from column 3).
        /// </summary>
        /// <param name="cellIdx">1-based index of Cell to return from existing Cells on this Row.</param>
        /// <returns>The Cell from this Row with the specified index among the existing Cells.</returns>
        public Cell GetCell(ushort cellIdx)
        {
            if (cellIdx < 1 || cellIdx > 256)
                throw new ArgumentOutOfRangeException(string.Format("cellIdx {0} must be between 1 and 256", cellIdx));

            if (cellIdx > _cells.Count)
                throw new ArgumentOutOfRangeException(
                    string.Format("cellIdx {0} is greater than the cell count {1}", cellIdx, _cells.Count));

            ushort idx = 1;
            foreach (ushort col in _cells.Keys)
            {
                if (idx == cellIdx)
                    return _cells[col];

                idx++;
            }

            throw new Exception(string.Format("Cell number {0} not found in row", cellIdx));
        }

        /// <summary>
        /// Gets the first column at which a Cell exists on this Row.
        /// </summary>
        public ushort MinCellCol
        {
            get { return _minCellCol; }
        }

        /// <summary>
        /// Gets the last column at which a Cell exists on this Row.
        /// </summary>
        public ushort MaxCellCol
        {
            get { return _maxCellCol; }
        }
    }
}
namespace Core.MyXls
{
    internal class RowBlocks
    {
        private Worksheet _worksheet;

        internal RowBlocks(Worksheet worksheet)
        {
            _worksheet = worksheet;
        }

        private ushort BlockCount
        {
            get
            {
                if (_worksheet.Rows.MaxRow == 0)
                    return 0;
                else
                {
                    ushort rowSpan = (ushort)(_worksheet.Rows.MaxRow - _worksheet.Rows.MinRow);
                    return (ushort)(((int)Math.Floor((decimal)rowSpan / 32)) + 1);
                }
            }
        }

        private Row GetBlockRow(ushort rbIdx, ushort brIdx)
        {
            Row blockRow = new Row();

            ushort j = 0;
            ushort row1 = (ushort)(_worksheet.Rows.MinRow + ((rbIdx - 1) * 32));

            CachedBlockRow cachedBlockRow = _worksheet.CachedBlockRow;

            ushort rowMin = row1;
            ushort rowMax = (ushort)(rowMin + 31);
            if (cachedBlockRow.RowBlockIndex == rbIdx)
            {
                if (cachedBlockRow.BlockRowIndex == brIdx)
                    return cachedBlockRow.Row;
                else if (brIdx < cachedBlockRow.BlockRowIndex)
                    //NOTE: !!! Is this right? vba said ".Row.Row" !!!
                    rowMax = cachedBlockRow.Row.RowIndex;
                else if (brIdx > cachedBlockRow.BlockRowIndex)
                {
                    if (cachedBlockRow.RowBlockIndex > 0)
                    {
                        rowMin = (ushort)(cachedBlockRow.Row.RowIndex + 1);
                        j = cachedBlockRow.BlockRowIndex;
                    }
                    else
                        rowMin = 1;
                }
            }

            for (ushort i = rowMin; i <= rowMax; i++)
            {
                if (_worksheet.Rows.RowExists(i))
                {
                    j++;
                    if (j == brIdx)
                    {
                        blockRow = _worksheet.Rows[i];
                        _worksheet.CachedBlockRow = new CachedBlockRow(rbIdx, brIdx, blockRow);
                    }
                }
            }

            return blockRow;
        }

        private ushort GetBlockRowCount(ushort idx)
        {
            ushort count = 0;

            ushort row1 = (ushort)(_worksheet.Rows.MinRow + ((idx - 1) * 32));
            for (ushort i = row1; i <= (row1 + 31); i++)
            {
                if (_worksheet.Rows.RowExists(i))
                    count++;
            }

            return count;
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                ushort j = BlockCount;
                int[] bLen = new int[j + 1];

                for (ushort i = 1; i <= j; i++)
                {
                    ushort m = GetBlockRowCount(i);
                    ushort[] cOff = new ushort[m + 1];
                    Bytes rows = new Bytes();
                    Bytes cells = new Bytes();
                    for (ushort k = 1; k <= m; k++)
                    {
                        if (k == 1)
                            cOff[k] = (ushort)((m - 1) * 20);
                        else if (k == 2)
                            cOff[k] = (ushort)cells.Length;
                        else
                            cOff[k] = (ushort)(cells.Length - cOff[k - 1]);

                        Row row = GetBlockRow(i, k);
                        rows.Append(ROW(row));
                        int o = row.CellCount;
                        for (ushort n = 1; n <= o; n++)
                        {
                            //OPTIM: The greatest time factor is the Row.Cell(x) lookup
                            Cell cell = row.GetCell(n);
                            cells.Append(cell.Bytes);
                        }
                    }
                    bytes.Append(rows);
                    bytes.Append(cells);
                    cOff[0] = (ushort)(rows.Length + cells.Length);
                    bLen[i] = bytes.Length;
                    bytes.Append(DBCELL(cOff));
                }

                _worksheet.DBCellOffsets = bLen;

                return bytes;
            }
        }

        private static Bytes ROW(Row row)
        {
            Bytes bytes = new Bytes();

            //Index of this row
            bytes.Append(BitConverter.GetBytes((ushort)(row.RowIndex - 1)));

            //Index to column of the first cell which is described by a cell record
            bytes.Append(BitConverter.GetBytes((ushort)(row.MinCellCol - 1)));

            //Index to column of the last cell which is described by a cell record, + 1
            bytes.Append(BitConverter.GetBytes(row.MaxCellCol));

            //Height of row in twips, custom row height indicator
            //TODO: Implement Row height and custom height indicators (excelfileformat.pdf p.190)
            bytes.Append(new byte[] { 0x08, 0x01 });

            //Not used
            bytes.Append(new byte[] { 0x00, 0x00 });

            //Not used anymore in BIFF8 (DBCELL instead)
            bytes.Append(new byte[] { 0x00, 0x00 });

            //Option flags and default row formatting
            //TODO: Implement Row option flags and default row formatting (excelfileformat.pdf p.190)
            bytes.Append(new byte[] { 0x00, 0x01, 0x0F, 0x00 });

            return Record.GetBytes(RID.ROW, bytes);
        }

        private static Bytes DBCELL(ushort[] cOff)
        {
            Bytes dbcell = new Bytes();

            for (int i = 0; i < cOff.Length; i++)
            {
                if (i == 0)
                    dbcell.Append(BitConverter.GetBytes((uint)cOff[i]));
                else
                    dbcell.Append(BitConverter.GetBytes(cOff[i]));
            }

            return Record.GetBytes(RID.DBCELL, dbcell);
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Represents and manages a collection of Row objects for a Worksheet.
    /// </summary>
    public class Rows : IEnumerable<Row>
    {
        private readonly SortedList<ushort, Row> _rows;

        private uint _minRow;
        private uint _maxRow;

        /// <summary>
        /// Initializes a new instance of the Rows object.
        /// </summary>
        public Rows()
        {
            _minRow = 0;
            _maxRow = 0;
            _rows = new SortedList<ushort, Row>();
        }

        /// <summary>
        /// Returns whether a Row exists at the specified index in this Collection.
        /// </summary>
        /// <param name="rowIdx">1-based index of Row to return from this collection.</param>
        /// <returns>The Row with the specified index from this collection.</returns>
        public bool RowExists(ushort rowIdx)
        {
            return _rows.ContainsKey(rowIdx);
        }

        /// <summary>
        /// Adds a Row at the specified row number.
        /// </summary>
        /// <param name="rowNum">1-based index of Row to add.</param>
        /// <returns>The Row added at the specified row number.</returns>
        public Row AddRow(ushort rowNum)
        {
            if (RowExists(rowNum))
                return _rows[rowNum];

            if (_minRow == 0)
            {
                _minRow = rowNum;
                _maxRow = rowNum;
            }
            else
            {
                if (rowNum < _minRow)
                    _minRow = rowNum;
                if (rowNum > _maxRow)
                    _maxRow = rowNum;
            }

            Row row = new Row();
            row.RowIndex = rowNum;

            _rows.Add(rowNum, row);

            return row;
        }

        /// <summary>
        /// Gets the count of Rows in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _rows.Count;
            }
        }

        /// <summary>
        /// Gets the Row from this collection with the specified row number.
        /// </summary>
        /// <param name="rowNumber">1-based row number to get.</param>
        /// <returns>Row at specified row number</returns>
        public Row this[ushort rowNumber]
        {
            get
            {
                if (!_rows.ContainsKey(rowNumber))
                    throw new Exception(string.Format("Row {0} not found", rowNumber));

                return _rows[rowNumber];
            }
        }

        /// <summary>
        /// Gets the smallest row number populated in this collection.  A Row is populated if it has any Cells,
        /// formatting or has been explicitly added.
        /// </summary>
        public uint MinRow
        {
            get { return _minRow; }
        }

        /// <summary>
        /// Gets the largest row number populated in this collection.  A Row is populated if it has any Cells,
        /// formatting or has been explicitly added.
        /// </summary>
        public uint MaxRow
        {
            get { return _maxRow; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Rows collection
        /// </summary>
        public IEnumerator<Row> GetEnumerator()
        {
            uint initialMinRow = MinRow;
            uint initialMaxRow = MaxRow;

            for (uint i = initialMinRow; i < initialMaxRow; i++)
            {
                if (initialMinRow != MinRow || initialMaxRow != MaxRow)
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                else yield return this[(ushort)i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// NOTE: This seems kludgy and wrong, but it was the easiest way
    /// I could think to allow for > int.MaxValue unique shared string 
    /// values.  (The index into the SST is a uint, so the max possible 
    /// unique BIFF8 allows for is uint.MaxValue.)
    /// </summary>
    internal class SharedStringTable
    {
        private readonly List<string> _stringsA = new List<string>();
        private readonly List<string> _stringsB = new List<string>();
        private readonly List<uint> _countsA = new List<uint>();
        private readonly List<uint> _countsB = new List<uint>();
        private bool _listAIsFull = false;
        private uint _countUnique = 0;
        private uint _countAll = 0;

        internal uint CountUnique
        {
            get { return _countUnique; }
        }

        internal uint Add(string sharedString)
        {
            _countAll++;
            int index = _stringsA.IndexOf(sharedString);
            if (index != -1)
            {
                _countsA[index]++;
                return (uint)index;
            }

            if (!_listAIsFull && _stringsA.Count == int.MaxValue)
                _listAIsFull = true;

            if (_listAIsFull)
            {
                index = _stringsB.IndexOf(sharedString);
                if (index != -1)
                {
                    _countsB[index]++;
                    return int.MaxValue + (uint)index;
                }
                else
                {
                    _stringsB.Add(sharedString);
                    _countsB.Add(1);
                    _countUnique++;
                    return int.MaxValue + (uint)(_stringsB.Count - 1);
                }
            }
            else
            {
                _stringsA.Add(sharedString);
                _countsA.Add(1);
                _countUnique++;
                return (uint)(_stringsA.Count - 1);
            }
        }

        internal string GetString(uint atIndex)
        {
            if (atIndex <= int.MaxValue)
                return _stringsA[(int)atIndex];
            else
            {
                atIndex -= int.MaxValue;
                return _stringsB[(int)atIndex];
            }
        }

        internal uint GetCount(uint atIndex)
        {
            if (atIndex <= int.MaxValue)
                return _countsA[(int)atIndex];
            else
            {
                atIndex -= int.MaxValue;
                return _countsB[(int)atIndex];
            }
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes sst = new Bytes();

                bool isFirstContinue = true;

                Bytes bytes = new Bytes();
                bytes.Append(BitConverter.GetBytes(_countUnique));
                bytes.Append(BitConverter.GetBytes(_countAll));

                int remainingRecordBytes = BIFF8.MaxDataBytesPerRecord - bytes.Length;

                AddStrings(_stringsA, ref remainingRecordBytes, ref bytes, sst, ref isFirstContinue);
                AddStrings(_stringsB, ref remainingRecordBytes, ref bytes, sst, ref isFirstContinue);

                //close out -- don't need to keep the new bytes ref
                Continue(sst, bytes, out remainingRecordBytes, ref isFirstContinue);

                return sst;
            }
        }

        //TODO: Implement Rich Text Value support
        //TODO: Implement Asian Text Value support
        internal void ReadBytes(Record sstRecord)
        {
            uint totalStrings = sstRecord.Data.Get(0, 4).GetBits().ToUInt32();
            uint uniqueStrings = sstRecord.Data.Get(4, 4).GetBits().ToUInt32();
            int stringIndex = 0;
            ushort offset = 8;
            int continueIndex = -1;
            while (stringIndex < uniqueStrings)
            {
                string sharedString = UnicodeBytes.Read(sstRecord, 16, ref continueIndex, ref offset);
                Add(sharedString);
                stringIndex++;
            }
            _countAll = totalStrings;
        }

        private void AddStrings(List<string> stringList, ref int remainingRecordBytes, ref Bytes bytes, Bytes sst, ref bool isFirstContinue)
        {
            foreach (string sharedString in stringList)
            {
                Bytes stringBytes = XlsDocument.GetUnicodeString(sharedString, 16);

                //per excelfileformat.pdf sec. 5.22, can't split a
                //Unicode string to another CONTINUE record before 
                //the first character's byte/s are written, and must 
                //repeat string option flags byte if it is split
                //OPTIM: For smaller filesize, handle the possibility of compressing continued portion of uncompressed strings (low ROI!)
                byte stringOptionFlag = 0xFF;
                bool charsAre16Bit = false;
                int minimumToAdd = int.MaxValue;

                if (stringBytes.Length > remainingRecordBytes)
                {
                    stringOptionFlag = stringBytes.Get(2, 1).ByteArray[0];
                    charsAre16Bit = (stringOptionFlag & 0x01) == 0x01;
                    minimumToAdd = charsAre16Bit ? 5 : 4;
                }

                while (stringBytes != null)
                {
                    if (stringBytes.Length > remainingRecordBytes) //add what we can and continue
                    {
                        bool stringWasSplit = false;
                        if (remainingRecordBytes > minimumToAdd)
                        {
                            int overLength = (stringBytes.Length - remainingRecordBytes);
                            bytes.Append(stringBytes.Get(0, remainingRecordBytes));
                            stringBytes = stringBytes.Get(remainingRecordBytes, overLength);
                            remainingRecordBytes -= remainingRecordBytes;
                            stringWasSplit = true;
                        }

                        bytes = Continue(sst, bytes, out remainingRecordBytes, ref isFirstContinue);

                        if (stringWasSplit)
                        {
                            bytes.Append(stringOptionFlag);
                            remainingRecordBytes--;
                        }
                    }
                    else //add what's left
                    {
                        bytes.Append(stringBytes);
                        remainingRecordBytes -= stringBytes.Length;
                        stringBytes = null; //exit loop to continue to next sharedString
                    }
                }
            }
        }

        //NOTE: Don't want to pass recordBytes by ref or when we set it to a new Bytes
        //instance, it will wipe out what was appended to bytes
        private Bytes Continue(Bytes sst, Bytes bytes, out int remainingRecordBytes, ref bool isFirstContinue)
        {
            sst.Append(Record.GetBytes(isFirstContinue ? RID.SST : RID.CONTINUE, bytes));

            remainingRecordBytes = BIFF8.MaxDataBytesPerRecord;
            isFirstContinue = false;
            return new Bytes();
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// The standard (built-in) Excel Format strings.
    /// NOTE: Currency_1 thru Currency_8 differ from the excelfileformat.pdf doc 
    /// by the added \'s (backslashes)
    /// </summary>
    public static class StandardFormats
    {
        /// <summary>General: General</summary>
        public static readonly string General = "General";

        /// <summary>Decimal 1: 0</summary>
        public static readonly string Decimal_1 = "0";

        /// <summary>Decimal 2: 0.00</summary>
        public static readonly string Decimal_2 = "0.00";

        /// <summary>Decimal 3: #,##0</summary>
        public static readonly string Decimal_3 = "#,##0";

        /// <summary>Decimal 4: #,##0.00</summary>
        public static readonly string Decimal_4 = "#,##0.00";

        /// <summary>Currency 1: "$"#,##0_);("$"#,##0)</summary>
        public static readonly string Currency_1 = "\"$\"#,##0_);\\(\"$\"#,##0\\)";

        /// <summary>Currency 2: "$"#,##0_);[Red]("$"#,##0)</summary>
        public static readonly string Currency_2 = "\"$\"#,##0_);[Red]\\(\"$\"#,##0\\)";

        /// <summary>Currency 3: "$"#,##0.00_);("$"#,##0.00)</summary>
        public static readonly string Currency_3 = "\"$\"#,##0.00_);\\(\"$\"#,##0.00\\)";

        /// <summary>Currency 4: "$"#,##0.00_);[Red]("$"#,##0.00)</summary>
        public static readonly string Currency_4 = "\"$\"#,##0.00_);[Red]\\(\"$\"#,##0.00\\)";

        /// <summary>Percent 1: 0%</summary>
        public static readonly string Percent_1 = "0%";

        /// <summary>Percent 2: 0.00%</summary>
        public static readonly string Percent_2 = "0.00%";

        /// <summary>Scientific 1: 0.00E+00</summary>
        public static readonly string Scientific_1 = "0.00E+00";

        /// <summary>Fraction 1: # ?/?</summary>
        public static readonly string Fraction_1 = "# ?/?";

        /// <summary>Fraction 2: # ??/??</summary>
        public static readonly string Fraction_2 = "# ??/??";

        /// <summary>Date 1: M/D/YY</summary>
        public static readonly string Date_1 = "M/D/YY";

        /// <summary>Date 2: D-MMM-YY</summary>
        public static readonly string Date_2 = "D-MMM-YY";

        /// <summary>Date 3: D-MMM</summary>
        public static readonly string Date_3 = "D-MMM";

        /// <summary>Date 4: MMM-YY</summary>
        public static readonly string Date_4 = "MMM-YY";

        /// <summary>Time 1: h:mm AM/PM</summary>
        public static readonly string Time_1 = "h:mm AM/PM";

        /// <summary>Time 2: h:mm:ss AM/PM</summary>
        public static readonly string Time_2 = "h:mm:ss AM/PM";

        /// <summary>Time 3: h:mm</summary>
        public static readonly string Time_3 = "h:mm";

        /// <summary>Time 4: h:mm:ss</summary>
        public static readonly string Time_4 = "h:mm:ss";

        /// <summary>Date/Time: M/D/YY h:mm</summary>
        public static readonly string Date_Time = "M/D/YY h:mm";

        /// <summary>Accounting 1: _(#,##0_);(#,##0)</summary>
        public static readonly string Accounting_1 = "_(#,##0_);(#,##0)";

        /// <summary>Accounting 2: _(#,##0_);[Red](#,##0)</summary>
        public static readonly string Accounting_2 = "_(#,##0_);[Red](#,##0)";

        /// <summary>Accounting 3: _(#,##0.00_);(#,##0.00)</summary>
        public static readonly string Accounting_3 = "_(#,##0.00_);(#,##0.00)";

        /// <summary>Accounting 4: _(#,##0.00_);[Red](#,##0.00)</summary>
        public static readonly string Accounting_4 = "_(#,##0.00_);[Red](#,##0.00)";

        /// <summary>Currency 5: _("$"* #,##0_);_("$"* (#,##0);_("$"* "-"_);_(@_)</summary>
        public static readonly string Currency_5 = "_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)";

        /// <summary>Currency 6: _(* #,##0_);_(* (#,##0);_(* "-"_);_(@_)</summary>
        public static readonly string Currency_6 = "_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)";

        /// <summary>Currency 7: _("$"* #,##0.00_);_("$"* (#,##0.00);_("$"* "-"??_);_(@_)</summary>
        public static readonly string Currency_7 = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";

        /// <summary>Currency 8: _(* #,##0.00_);_(* (#,##0.00);_(* "-"??_);_(@_)</summary>
        public static readonly string Currency_8 = "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)";

        /// <summary>Time 5: mm:ss</summary>
        public static readonly string Time_5 = "mm:ss";

        /// <summary>Time 6: [h]:mm:ss</summary>
        public static readonly string Time_6 = "[h]:mm:ss";

        /// <summary>Time 7: mm:ss.0</summary>
        public static readonly string Time_7 = "mm:ss.0";

        /// <summary>Scientific 2: ##0.0E+0</summary>
        public static readonly string Scientific_2 = "##0.0E+0";

        /// <summary>Text: @</summary>
        public static readonly string Text = "@";

        /// <summary>
        /// 57
        /// </summary>
        public static readonly string Date_5 = "yyyy年m月";

        /// <summary>
        /// 58
        /// </summary>
        public static readonly string Date_6 = "m月d日";

    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Represents a Style in Excel.
    /// </summary>
    public class Style : ICloneable
    {
        //TODO: Finish Style implementation
        private XlsDocument _doc;
        private XF _xf;

        private bool _isInitializing = false;

        private ushort? _id = null;

        internal Style(XlsDocument doc, XF xf)
        {
            _isInitializing = true;

            _doc = doc;
            _xf = xf;

            _isInitializing = false;
        }

        private void OnChange()
        {
            if (_isInitializing)
                return;

            _id = null;
            _xf.OnChange();
        }

        /// <summary>
        /// Gets the ID value of this Style object.
        /// </summary>
        public ushort ID
        {
            get
            {
                if (_id == null)
                    _id = _doc.Workbook.Styles.Add(this);

                return (ushort)_id;
            }
        }

        /// <summary>
        /// Returns whether a given Style object is value-equal to this Style object.
        /// </summary>
        /// <param name="that">Another Style object to compare to this Style object.</param>
        /// <returns>true if that Style object is value-equal to this Style object,
        /// false otherwise.</returns>
        public bool Equals(Style that)
        {
            //TODO: Add comparisons when Class members are added
            return true;
        }

        internal Bytes Bytes
        {
            get
            {
                throw new NotImplementedException();
                //				Doc.GetRecBin(RID.STYLE, new byte[] {0x10, 0x80, 0x03, 0xFF});
            }
        }

        #region ICloneable members

        /// <summary>
        /// Returns a new Style object which is value-equal to this Style object.
        /// </summary>
        /// <returns>A new Style object which is value-equal to this Style object.</returns>
        public object Clone()
        {
            Style clone = new Style(this._doc, this._xf);

            //TODO: Add as properties are added to class.
            clone._doc = this._doc;

            return clone;
        }

        #endregion
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Represents and manages a collection of Style objects for a Workbook.
    /// </summary>
    public class Styles
    {
        private readonly XlsDocument _doc;

        private List<Style> _styles = null;

        /// <summary>
        /// Initializes a new instance of the Styles object for the give XlsDocument.
        /// </summary>
        /// <param name="doc">The parent XlsDocument object for the new Styles object.</param>
        public Styles(XlsDocument doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Gets a count of the Style objects currently in this collection.
        /// </summary>
        public int Count
        {
            get { return _styles.Count; }
        }

        /// <summary>
        /// Adds a Style object to this collection and returns the Style object's
        /// id value.
        /// </summary>
        /// <param name="style">The Style object to add to this collection.</param>
        /// <returns>The id value of the given Style object which has been added
        /// to this collection.</returns>
        public ushort Add(Style style)
        {
            ushort? id = GetID(style);

            if (id == null)
            {
                if (_styles == null)
                    _styles = new List<Style>();

                id = (ushort)_styles.Count;
                _styles.Add((Style)style.Clone());
            }

            return (ushort)id;
        }

        /// <summary>
        /// Gets whether the given Style object exists in this collection and so will be
        /// written to the XlsDocument.
        /// </summary>
        /// <param name="style">The Style object which is to be checked whether it exists
        /// in this collection.</param>
        /// <returns>true if the given Style object exists in this collection, false otherwise.</returns>
        public bool IsWritten(Style style)
        {
            return (GetID(style) != null);
        }

        /// <summary>
        /// Returns the ID of a given Style object in this collection.
        /// </summary>
        /// <param name="style">The Style object whose ID is to be returned.</param>
        /// <returns>The ID of the given Style object in this collection.</returns>
        public ushort? GetID(Style style)
        {
            ushort? id = null;

            if (_styles == null)
                return id;

            for (ushort i = 0; i < _styles.Count; i++)
            {
                Style styleItem = _styles[i];
                if (styleItem.Equals(style))
                {
                    id = i;
                    break;
                }
            }

            return id;
        }

        /// <summary>
        /// Gets the Style object from this collection at the specified index.
        /// </summary>
        /// <param name="index">The index in this collection from which to get a Style object.</param>
        /// <returns>The Style object from this collection at the specified index.</returns>
        public Style this[int index]
        {
            get
            {
                return (Style)_styles[index].Clone();
            }
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                foreach (Style style in _styles)
                {
                    bytes.Append(style.Bytes);
                }

                return bytes;
            }
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Indicates local culture's text direction (left-to-right or right-to-left).
    /// </summary>
    public enum TextDirections : ushort
    {
        /// <summary>Default - By Context</summary>
        Default = ByContext,

        /// <summary>By Context</summary>
        ByContext = 0,

        /// <summary>Left to Right</summary>
        LeftToRight = 1,

        /// <summary>Right to Left</summary>
        RightToLeft = 2
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Indicates text rotation.
    /// </summary>
    public enum TextRotations : int
    {
        /// <summary>Default TextRotations value (0 degrees).</summary>
        Default = 0,
        /// <summary>Letters are stacked top-to-bottom, but not rotated</summary>
        Stacked = 255
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Underline types available in an Excel document.
    /// </summary>
    public enum UnderlineTypes : byte
    {
        /// <summary>Default - None</summary>
        Default = None,

        /// <summary>None</summary>
        None = 0x00,

        /// <summary>Single</summary>
        Single = 0x01,

        /// <summary>Double</summary>
        Double = 0x02,

        /// <summary>Single Accounting</summary>
        SingleAccounting = 0x21,

        /// <summary>Double Accounting</summary>
        DoubleAccounting = 0x22
    }
}
namespace Core.MyXls
{
    internal class UnicodeBytes
    {
        internal static void Write(string text, Bytes bytes)
        {
            ushort offset = 0;
            Write(text, bytes, ref offset);
        }

        internal static void Write(string text, Bytes bytes, ref ushort offset)
        {
            throw new NotImplementedException();
        }

        internal static string Read(Bytes bytes, int lengthBits)
        {
            Record record = new Record(RID.Empty, bytes);
            return Read(record, lengthBits, 0);
        }

        private static string Read(Record record, int lengthBits, ushort offset)
        {
            int continueIndex = -1; //throw away
            return Read(record, lengthBits, ref continueIndex, ref offset);
        }

        internal static string Read(Record record, int lengthBits, ref int continueIndex, ref ushort offset)
        {
            string text = string.Empty;

            ReadState state = new ReadState(record, lengthBits, continueIndex, offset);
            Read(state);
            continueIndex = state.ContinueIndex;
            offset = state.Offset;

            return new string(state.CharactersRead.ToArray());
        }

        private static void Read(ReadState state)
        {
            Bytes data = state.GetRecordData();

            bool compressed = (state.OptionsFlags & 0x01) == 0;

            ushort bytesAvailable = (ushort)(data.Length - state.Offset);

            ushort bytesToRead;
            if (state.CharactersRead.Count < state.TotalCharacters)
            {
                ushort charBytesRemaining = (ushort)(state.TotalCharacters - state.CharactersRead.Count);
                if (!compressed)
                    charBytesRemaining *= 2;

                if (bytesAvailable < charBytesRemaining)
                    bytesToRead = bytesAvailable;
                else
                    bytesToRead = charBytesRemaining;

                byte[] charBytes = data.Get(state.Offset, bytesToRead).ByteArray;

                if (compressed)
                {
                    //decompress
                    byte[] wideBytes = new byte[charBytes.Length * 2];
                    for (int i = 0; i < charBytes.Length; i++)
                        wideBytes[2 * i] = charBytes[i];
                    charBytes = wideBytes;
                }
                state.Offset += bytesToRead;
                bytesAvailable -= bytesToRead;

                state.CharactersRead.AddRange(Encoding.Unicode.GetChars(charBytes));
            }

            bool allCharsRead = state.CharactersRead.Count == state.TotalCharacters;

            if (state.HasRichTextSettings && bytesAvailable > 0 && allCharsRead &&
                state.FormattingRunBytes.Count < (state.FormattingRunCount * 4))
            {
                bytesToRead = Math.Min(bytesAvailable, (ushort)Math.Min(state.FormattingRunCount * 4, ushort.MaxValue));
                state.FormattingRunBytes.AddRange(data.Get(state.Offset, bytesToRead).ByteArray);
                state.Offset += bytesToRead;
                bytesAvailable -= bytesToRead;
            }

            if (state.HasAsianPhonetics && bytesAvailable > 0 && allCharsRead &&
                state.PhoneticSettingsBytes.Count < state.PhoneticSettingsByteCount)
            {
                bytesToRead = Math.Min(bytesAvailable, (ushort)Math.Min(state.PhoneticSettingsByteCount, ushort.MaxValue));
                state.PhoneticSettingsBytes.AddRange(data.Get(state.Offset, bytesToRead).ByteArray);
                state.Offset += bytesToRead;
                bytesAvailable -= bytesToRead;
            }

            if (state.CharactersRead.Count < state.TotalCharacters ||
                state.FormattingRunBytes.Count < (state.FormattingRunCount * 4) ||
                state.PhoneticSettingsBytes.Count < state.PhoneticSettingsByteCount)
            {
                state.Continue(true);
                Read(state);
            }
            else if (bytesAvailable == 0 && (state.ContinueIndex + 1) < state.Record.Continues.Count)
            {
                state.Continue(false);
            }
        }

        private class ReadState
        {
            public Record Record;
            public int LengthBits;
            public ushort TotalCharacters = 0;
            public int ContinueIndex;
            public ushort Offset;
            public List<char> CharactersRead = new List<char>();
            public bool HasAsianPhonetics = false;
            public bool HasRichTextSettings = false;
            public ushort FormattingRunCount = 0;
            public List<byte> FormattingRunBytes = new List<byte>();
            public uint PhoneticSettingsByteCount = 0;
            public List<byte> PhoneticSettingsBytes = new List<byte>();
            public byte OptionsFlags = 0x00;

            public ReadState(Record record, int lengthBits, int continueIndex, ushort offset)
            {
                LengthBits = lengthBits;
                Record = record;
                ContinueIndex = continueIndex;
                Offset = offset;

                Bytes data = GetRecordData();

                if (LengthBits == 8)
                {
                    TotalCharacters = data.Get(offset, 1).ByteArray[0];
                    Offset++;
                }
                else
                {
                    TotalCharacters = data.Get(offset, 2).GetBits().ToUInt16();
                    Offset += 2;
                }

                ReadOptionsFlags();

                HasAsianPhonetics = (OptionsFlags & 0x04) == 0x04;
                HasRichTextSettings = (OptionsFlags & 0x08) == 0x08;

                if (HasRichTextSettings)
                {
                    FormattingRunCount = BitConverter.ToUInt16(data.Get(Offset, 2).ByteArray, 0);
                    Offset += 2;
                    //bytesRemaining += (ushort)(4 * formattingRuns);
                    //NOTE: When implementing Rich Text, remember to add total length of formating runs to bytesRead
                    //throw new NotSupportedException("Rich Text text values in cells are not yet supported");
                }

                if (HasAsianPhonetics)
                {
                    PhoneticSettingsByteCount = BitConverter.ToUInt32(data.Get(Offset, 4).ByteArray, 0);
                    Offset += 4;
                    //NOTE: When implementing Asian Text, remember to add total length of Asian Phonetic Settings Block to bytesRead
                    //throw new NotSupportedException("Asian Phonetic text values in cells are not yet supported");
                }
            }

            private void ReadOptionsFlags()
            {
                OptionsFlags = GetRecordData().Get(Offset++, 1).ByteArray[0];
            }

            public Bytes GetRecordData()
            {
                if (ContinueIndex == -1)
                    return Record.Data;
                else
                    return Record.Continues[ContinueIndex].Data;
            }

            public void Continue(bool readOptions)
            {
                ContinueIndex++;
                Offset = 0;
                if (readOptions)
                {
                    ReadOptionsFlags();
                }
            }
        }
    }
}
namespace Core.MyXls
{
    internal static class Util
    {
        internal static void ValidateUShort(int theInt, string fieldName)
        {
            if (theInt < ushort.MinValue || theInt > ushort.MaxValue)
                throw new ArgumentException(string.Format("{0} value {1} must be between 1 and {2}", fieldName, theInt, ushort.MaxValue - 1));
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Vertical alignments available in an Excel document.
    /// </summary>
    public enum VerticalAlignments : byte
    {
        /// <summary>Default - Bottom</summary>
        Default = Bottom,

        /// <summary>Top</summary>
        Top = 0,

        /// <summary>Centered</summary>
        Centered = 1,

        /// <summary>Bottom</summary>
        Bottom = 2,

        /// <summary>Justified</summary>
        Justified = 3,

        /// <summary>Distributed</summary>
        Distributed = 4
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Another main class in an XlsDocument.  The Workbook holds all Worksheets, and properties
    /// or settings global to all Worksheets.  It also holds the Fonts, Formats, Styles and XFs
    /// collections.
    /// </summary>
    public class Workbook
    {
        private readonly XlsDocument _doc;

        private readonly Worksheets _worksheets;
        private readonly Fonts _fonts;
        private readonly Formats _formats;
        private readonly Styles _styles;
        private readonly XFs _xfs;
        private readonly Palette _palette;
        private readonly SharedStringTable _sharedStringTable = new SharedStringTable();

        private bool _shareStrings = false;

        private bool _protectContents = false;
        private bool _protectWindowSettings = false;
        private string _password = string.Empty;
        private bool _protectRevisions = false;
        private string _revisionsPassword = string.Empty;

        internal Workbook(XlsDocument doc)
        {
            _doc = doc;

            _worksheets = new Worksheets(_doc);
            _fonts = new Fonts(_doc);
            _formats = new Formats(_doc);
            _styles = new Styles(_doc);
            _xfs = new XFs(_doc, this);
            _palette = new Palette(this);
        }

        internal Workbook(XlsDocument doc, Bytes bytes, BytesReadCallback bytesReadCallback)
            : this(doc)
        {
            ReadBytes(bytes, bytesReadCallback);
        }

        /// <summary>
        /// Gets the Worksheets collection for this Workbook.
        /// </summary>
        public Worksheets Worksheets
        {
            get { return _worksheets; }
        }

        /// <summary>
        /// Gets the Fonts collection for this Workbook.
        /// </summary>
        public Fonts Fonts
        {
            get { return _fonts; }
        }

        /// <summary>
        /// Gets the Formats collection for this Workbook.
        /// </summary>
        public Formats Formats
        {
            get { return _formats; }
        }

        /// <summary>
        /// Gets the Styles collection for this workbook.
        /// </summary>
        public Styles Styles
        {
            get { return _styles; }
        }

        /// <summary>
        /// Gets or sets whether the contents of this Workbook's Worksheets
        /// are protected (Adding/Removing/Reordering Worksheets, etc.).
        /// </summary>
        public bool ProtectContents
        {
            get { return _protectContents; }
            set { _protectContents = value; }
        }

        /// <summary>
        /// Gets or sets whether this Workbook's Window settings are protected
        /// (Un/Freezing panes, etc.).
        /// </summary>
        public bool ProtectWindowSettings
        {
            get { return _protectWindowSettings; }
            set { _protectWindowSettings = value; }
        }

        /// <summary>
        /// Gets or sets whether this Workbook will optimize for smaller file 
        /// size by utilizing a SharedStringTable for text values.
        /// </summary>
        public bool ShareStrings
        {
            get { return _shareStrings; }
            set { _shareStrings = value; }
        }

        internal SharedStringTable SharedStringTable
        {
            get { return _sharedStringTable; }
        }

        internal XFs XFs
        {
            get { return _xfs; }
        }

        internal Palette Palette
        {
            get { return _palette; }
        }

        internal Bytes Bytes
        {
            get
            {
                if (_worksheets.Count == 0)
                    _worksheets.Add("Sheet1");

                Bytes bytesA = new Bytes(); //BOF (inclusive) to 1st BOUNDSHEET (exclusive)
                Bytes bytesB = new Bytes(); //BOUNDSHEETs
                Bytes bytesC = new Bytes(); //BOUNDSHEET (exclusive) to EOF (inclusive)
                Bytes bytesD = new Bytes(); //Worksheet Streams

                //TODO: Break this down to component option bits (BOF Function/Class?)
                bytesA.Append(Record.GetBytes(RID.BOF, new byte[] { 0x00, 0x06, 0x05, 0x00, 0xAF, 0x18, 0xCD, 0x07, 0xC9, 0x40, 0x00, 0x00, 0x06, 0x01, 0x00, 0x00 }));

                //<Workbook Protection Block>
                if (_protectContents)
                    bytesA.Append(Record.GetBytes(RID.PROTECT, new byte[] { 0x01, 0x00 }));
                if (_protectWindowSettings)
                    bytesA.Append(Record.GetBytes(RID.WINDOWPROTECT, new byte[] { 0x01, 0x00 }));
                //</Workbook Protection Block>

                //TODO: Break this down to component option bits (WINDOW1 Function/Class?)
                bytesA.Append(Record.GetBytes(RID.WINDOW1, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x58, 0x02 }));

                //TODO: Implement private functions for these
                bytesA.Append(_fonts.Bytes);
                bytesA.Append(_formats.Bytes);
                bytesA.Append(_xfs.Bytes);
                bytesA.Append(Record.GetBytes(RID.STYLE, new byte[] { 0x10, 0x80, 0x00, 0xFF })); //STYLE

                if (SharedStringTable.CountUnique > 0)
                    bytesC.Append(SharedStringTable.Bytes);
                bytesC.Append(Record.GetBytes(RID.EOF, new byte[0])); //EOF

                int basePosition = bytesA.Length + bytesC.Length;
                for (int i = 0; i < _worksheets.Count; i++)
                    basePosition += XlsDocument.GetUnicodeString(_worksheets[i].Name, 8).Length + 10; //Add length of BOUNDSHEET Records

                _worksheets.StreamOffset = basePosition;

                for (int i = 0; i < _worksheets.Count; i++)
                {
                    if (i > 0)
                        basePosition += _worksheets[i - 1].StreamByteLength;

                    Worksheet sheet = _worksheets[i];
                    bytesB.Append(BOUNDSHEET(sheet, basePosition));
                    bytesD.Append(sheet.Bytes);
                }

                bytesA.Append(bytesB);
                bytesA.Append(bytesC);
                bytesA.Append(bytesD);

                if (_doc.ForceStandardOle2Stream)
                    bytesA = _doc.GetStandardOLE2Stream(bytesA);

                return bytesA;
            }
        }

        #region For Unit Testing Only
        internal delegate void BytesReadCallback(List<Record> records);
        #endregion

        private void ReadBytes(Bytes bytes, BytesReadCallback bytesReadCallback)
        {
            try
            {
                if (bytes == null)
                    throw new ArgumentNullException("bytes");

                if (bytes.Length == 0)
                    throw new ArgumentException("can't be zero-length", "bytes");

                //The XF's read in won't necessarily have the same ID (index) once added to this Workbook,
                //so we need to keep the cross-reference list for re-assignment as we read in the cell records later
                SortedList<ushort, ushort> xfIdLookups = new SortedList<ushort, ushort>();

                List<Record> records = Record.GetAll(bytes);

                List<Record> fontRecords = new List<Record>();
                List<Record> formatRecords = new List<Record>();
                List<Record> xfRecords = new List<Record>();
                List<Record> boundSheetRecords = new List<Record>();
                Record sstRecord = Record.Empty;

                SortedList<int, List<Record>> sheetRecords = new SortedList<int, List<Record>>();

                int sheetIndex = -1;

                foreach (Record record in records)
                {
                    if (sheetIndex >= 0)
                    {
                        if (!sheetRecords.ContainsKey(sheetIndex))
                            sheetRecords[sheetIndex] = new List<Record>();
                        sheetRecords[sheetIndex].Add(record);
                        if (record.RID == RID.EOF)
                            sheetIndex++;
                    }
                    else if (record.RID == RID.FONT)
                        fontRecords.Add(record);
                    else if (record.RID == RID.FORMAT)
                        formatRecords.Add(record);
                    else if (record.RID == RID.XF)
                        xfRecords.Add(record);
                    else if (record.RID == RID.BOUNDSHEET)
                        boundSheetRecords.Add(record);
                    else if (record.RID == RID.SST)
                        sstRecord = record;
                    else if (record.RID == RID.EOF)
                        sheetIndex++;
                }

                SortedList<ushort, Font> fonts = new SortedList<ushort, Font>();
                SortedList<ushort, string> formats = new SortedList<ushort, string>();
                SortedList<ushort, XF> xfs = new SortedList<ushort, XF>();

                ushort index = 0;
                foreach (Record record in fontRecords)
                {
                    Font font = new Font(_doc, record.Data);
                    fonts[index++] = font;
                    this.Fonts.Add(font);
                }

                foreach (Record record in formatRecords)
                {
                    Bytes recordData = record.Data;
                    string format = UnicodeBytes.Read(recordData.Get(2, recordData.Length - 2), 16);
                    index = BitConverter.ToUInt16(recordData.Get(2).ByteArray, 0);
                    formats[index] = format;
                    this.Formats.Add(format);
                }

                index = 0;
                for (index = 0; index < xfRecords.Count; index++)
                {
                    Record record = xfRecords[index];
                    Bytes recordData = record.Data;
                    ushort fontIndex = BitConverter.ToUInt16(recordData.Get(0, 2).ByteArray, 0);
                    ushort formatIndex = BitConverter.ToUInt16(recordData.Get(2, 2).ByteArray, 0);
                    //ushort styleIndex = BitConverter.ToUInt16(recordData.Get(4, 2))
                    if (!fonts.ContainsKey(fontIndex))
                        continue; //TODO: Perhaps default to default XF?  NOTE: This is encountered with TestReferenceFile BlankBudgetWorksheet.xls
                    Font font = fonts[fontIndex];
                    string format;
                    if (formats.ContainsKey(formatIndex))
                        format = formats[formatIndex];
                    else if (_formats.ContainsKey(formatIndex))
                        format = _formats[formatIndex];
                    else
                        throw new ApplicationException(string.Format("Format {0} not found in read FORMAT records or standard/default FORMAT records.", formatIndex));
                    xfIdLookups[index] = this.XFs.Add(new XF(_doc, record.Data, font, format));
                }
                this.XFs.XfIdxLookups = xfIdLookups;

                if (sstRecord != Record.Empty)
                    this.SharedStringTable.ReadBytes(sstRecord);

                if (bytesReadCallback != null)
                    bytesReadCallback(records);

                for (int i = 0; i < boundSheetRecords.Count; i++)
                {
                    _worksheets.Add(boundSheetRecords[i], sheetRecords[i]);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static Bytes BOUNDSHEET(Worksheet sheet, int basePosition)
        {
            Bytes bytes = new Bytes();

            Bytes sheetName = XlsDocument.GetUnicodeString(sheet.Name, 8);
            bytes.Append(WorksheetVisibility.GetBytes(sheet.Visibility));
            bytes.Append(WorksheetType.GetBytes(sheet.SheetType));
            bytes.Append(sheetName);
            bytes.Prepend(BitConverter.GetBytes((int)basePosition)); //TODO: this should probably be unsigned 32 instead

            bytes.Prepend(BitConverter.GetBytes((ushort)bytes.Length));
            bytes.Prepend(RID.BOUNDSHEET);

            return bytes;
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// A main class for an XlsDocument, representing one Worksheet in the Workbook.  The 
    /// Worksheet holds the Cells in its Rows.
    /// </summary>
    public class Worksheet
    {
        private readonly XlsDocument _doc;

        private readonly List<ColumnInfo> _columnInfos = new List<ColumnInfo>();
        public List<ColumnInfo> ColumnInfos
        {
            get
            {
                return _columnInfos;
            }
        }
        private readonly List<MergeArea> _mergeAreas = new List<MergeArea>();

        private readonly Cells _cells;
        private readonly Rows _rows;
        private readonly RowBlocks _rowBlocks;

        private WorksheetVisibilities _visibility;
        private WorksheetTypes _sheettype;
        private string _name;
        private int _streamByteLength;
        private int[] _dbCellOffsets;
        private bool _protected = false;

        private CachedBlockRow _cachedBlockRow;

        internal Worksheet(XlsDocument doc)
        {
            _doc = doc;

            _visibility = WorksheetVisibilities.Default;
            _sheettype = WorksheetTypes.Default;
            _streamByteLength = 0;

            _dbCellOffsets = new int[0];

            _cells = new Cells(this);
            _rows = new Rows();
            _rowBlocks = new RowBlocks(this);

            _cachedBlockRow = CachedBlockRow.Empty;

            _columnInfos = new List<ColumnInfo>();
        }

        internal Worksheet(XlsDocument doc, Record boundSheet, List<Record> sheetRecords)
            : this(doc)
        {
            byte[] byteArray = boundSheet.Data.ByteArray;

            byte visibility = byteArray[4];
            if (visibility == 0x00)
                _visibility = WorksheetVisibilities.Visible;
            else if (visibility == 0x01)
                _visibility = WorksheetVisibilities.Hidden;
            else if (visibility == 0x02)
                _visibility = WorksheetVisibilities.StrongHidden;
            else
                throw new ApplicationException(string.Format("Unknown Visibility {0}", visibility));

            byte type = byteArray[5];
            if (type == 0x00)
                _sheettype = WorksheetTypes.Worksheet;
            else if (type == 0x02)
                _sheettype = WorksheetTypes.Chart;
            else if (type == 0x06)
                _sheettype = WorksheetTypes.VBModule;
            else
                throw new ApplicationException(string.Format("Unknown Sheet Type {0}", type));

            List<Record> rowRecords = new List<Record>();
            List<Record> cellRecords = new List<Record>();

            for (int i = 0; i < sheetRecords.Count; i++)
            {
                Record record = sheetRecords[i];
                if (record.IsCellRecord())
                {
                    if (record.RID == RID.FORMULA)
                    {
                        Record formulaStringRecord = null;
                        if ((i + i) < sheetRecords.Count)
                        {
                            formulaStringRecord = sheetRecords[i + 1];
                            if (formulaStringRecord.RID != RID.STRING)
                                formulaStringRecord = null;
                        }
                        record = new FormulaRecord(record, formulaStringRecord);
                    }

                    cellRecords.Add(record);
                }
                else if (record.RID == RID.ROW)
                    rowRecords.Add(record);
            }

            //Add the Rows first so they exist for adding the Cells
            foreach (Record rowRecord in rowRecords)
            {
                Bytes rowBytes = rowRecord.Data;
                ushort rowIndex = rowBytes.Get(0, 2).GetBits().ToUInt16();
                Row row = Rows.AddRow(rowIndex);
                bool isDefaultHeight = rowBytes.Get(6, 2).GetBits().Values[15];
                ushort height = 0;
                if (!isDefaultHeight)
                {
                    height = rowBytes.Get(6, 2).GetBits().Get(0, 14).ToUInt16();
                    //TODO: Set height on Row when reading (after Row Height implemented)
                }
                bool defaultsWritten = (rowBytes.Get(10, 1).ByteArray[0] == 0x01);
                if (defaultsWritten)
                {
                    //TODO: Read ROW record defaults
                }
            }

            foreach (Record record in cellRecords)
                AddCells(record);

            _name = UnicodeBytes.Read(boundSheet.Data.Get(6, boundSheet.Data.Length - 6), 8);
        }

        private void AddCells(Record record)
        {
            Bytes bytes = record.Data;
            ushort rowIndex = bytes.Get(0, 2).GetBits().ToUInt16();
            ushort colIndex = bytes.Get(2, 2).GetBits().ToUInt16();
            ushort lastColIndex = colIndex;
            ushort offset = 4;

            byte[] rid = record.RID;
            bool isMulti = false;

            if (rid == RID.MULBLANK)
            {
                isMulti = true;
                rid = RID.BLANK;
            }
            else if (rid == RID.MULRK)
            {
                isMulti = true;
                rid = RID.RK;
            }

            if (isMulti)
                lastColIndex = bytes.Get(bytes.Length - 2, 2).GetBits().ToUInt16();


            while (colIndex <= lastColIndex)
            {
                Cell cell = Cells.Add((ushort)(rowIndex + 1), (ushort)(colIndex + 1));
                ushort xfIndex = bytes.Get(offset, 2).GetBits().ToUInt16();
                offset += 2;

                Bytes data;
                if (rid == RID.BLANK)
                    data = new Bytes();
                else if (rid == RID.RK)
                {
                    data = bytes.Get(offset, 4);
                    offset += 4;
                    cell.SetValue(rid, data);
                }
                else
                {
                    data = bytes.Get(offset, bytes.Length - offset);
                    if (rid == RID.FORMULA)
                    {
                        FormulaRecord formulaRecord = record as FormulaRecord;
                        cell.SetFormula(data, formulaRecord.StringRecord);
                    }
                    else
                        cell.SetValue(rid, data);
                }
                colIndex++;
            }
        }

        internal XlsDocument Document
        {
            get { return _doc; }
        }

        /// <summary>
        /// Gets or sets the Name of this Worksheet.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the Visibility of this Worksheet.
        /// </summary>
        public WorksheetVisibilities Visibility
        {
            get { return _visibility; }
            set { _visibility = value; }
        }

        /// <summary>
        /// Gets or sets the Worksheet Type of this Worksheet.
        /// </summary>
        public WorksheetTypes SheetType
        {
            get { return _sheettype; }
            set { _sheettype = value; }
        }

        /// <summary>
        /// Gets the Cells collection of this Worksheet.
        /// </summary>
        public Cells Cells
        {
            get { return _cells; }
        }

        /// <summary>
        /// Gets the Rows collection of this Worksheet.
        /// </summary>
        public Rows Rows
        {
            get { return _rows; }
        }

        /// <summary>
        /// Gets or sets whether this Worksheet is Protected (the Locked/Hidden status 
        /// of objects such as cells is enforced by Excel).
        /// </summary>
        public bool Protected
        {
            get { return _protected; }
            set { _protected = value; }
        }

        internal int StreamByteLength
        {
            get { return _streamByteLength; }
        }

        internal int[] DBCellOffsets
        {
            set { _dbCellOffsets = value; }
        }

        internal Bytes Bytes
        {
            get
            {
                //NOTE: See excelfileformat.pdf Sec. 4.2

                Bytes bytesA = new Bytes();
                Bytes bytesB = new Bytes();
                Bytes bytesC = new Bytes();
                Bytes bytesD = new Bytes();
                Bytes bytesE = new Bytes();
                //                int defaultColumnWidthOffset = 0;
                int rowBlock1Offset;

                //TODO: Move the rest of these sections to private functions like WINDOW2
                bytesD.Append(_rowBlocks.Bytes); //This is first so the RowBlocks interface can calculate the
                //DBCELLOffsets array used below

                //bytesA = BOF (inc) to INDEX (exc)
                //bytesB = INDEX
                //bytesC = INDEX (exc) to RowBlocks (exc)
                //bytesD = RowBlocks
                //bytesE = RowBlocks (exc) to EOF (inc)

                bytesA.Append(Record.GetBytes(RID.BOF, new byte[] { 0x00, 0x06, 0x10, 0x00, 0xAF, 0x18, 0xCD, 0x07, 0xC1, 0x40, 0x00, 0x00, 0x06, 0x01, 0x00, 0x00 }));

                if (_protected)
                    bytesC.Append(Record.GetBytes(RID.PROTECT, new byte[] { 0x01, 0x00 }));
                bytesC.Append(COLINFOS()); //Out of order for a reason - see rowblock1offset calc below

                rowBlock1Offset = _doc.Workbook.Worksheets.StreamOffset;
                int j = _doc.Workbook.Worksheets.GetIndex(Name);
                for (int i = 1; i < j; i++)
                    rowBlock1Offset += _doc.Workbook.Worksheets[i].StreamByteLength;
                rowBlock1Offset += bytesA.Length + (20 + (4 * (_dbCellOffsets.Length - 1))) + bytesC.Length;

                bytesB.Append(INDEX(rowBlock1Offset));

                //BEGIN Worksheet View Settings Block
                bytesE.Append(WINDOW2());
                //END Worksheet View Settings Block

                bytesE.Append(MERGEDCELLS());
                bytesE.Append(Record.GetBytes(RID.EOF, new byte[0]));

                bytesA.Append(bytesB);
                bytesA.Append(bytesC);
                bytesA.Append(bytesD);
                bytesA.Append(bytesE);

                _streamByteLength = bytesA.Length;

                return bytesA;
            }
        }

        private Bytes INDEX(int baseLength)
        {
            Bytes index = new Bytes();

            //Not used
            index.Append(new byte[] { 0x00, 0x00, 0x00, 0x00 });

            //Index to first used row (0-based)
            index.Append(BitConverter.GetBytes(_rows.MinRow - 1));

            //Index to first row of unused tail of sheet(last row + 1, 0-based)
            index.Append(BitConverter.GetBytes(_rows.MaxRow));

            //Absolute stream position of the DEFCOLWIDTH record
            //TODO: Implement Worksheet.INDEX Absolute stream position of the DEFCOLWIDTH record (not necessary)
            index.Append(BitConverter.GetBytes((uint)0));

            for (int i = 1; i < _dbCellOffsets.Length; i++)
                index.Append(BitConverter.GetBytes((uint)(baseLength + _dbCellOffsets[i])));

            return Record.GetBytes(RID.INDEX, index);
        }

        private Bytes WINDOW2()
        {
            Bytes window2 = new Bytes();

            //TODO: Implement options - excelfileformat.pdf pp.210-211
            if (_doc.Workbook.Worksheets.GetIndex(Name) == 0) //NOTE: This was == 1, but the base of the worksheets collection must have changed
                window2.Append(new byte[] { 0xB6, 0x06 });
            else
                window2.Append(new byte[] { 0xB6, 0x04 });
            window2.Append(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

            return Record.GetBytes(RID.WINDOW2, window2);
        }

        private Bytes COLINFOS()
        {
            Bytes colinfos = new Bytes();

            for (int i = 0; i < _columnInfos.Count; i++)
                colinfos.Append(_columnInfos[i].Bytes);

            return colinfos;
        }

        private Bytes MERGEDCELLS()
        {
            Bytes mergedcells = new Bytes();

            int areaIndex = 0;
            int mergeAreaCount = _mergeAreas.Count;
            long areasPerRecord = 1027;
            int recordsRequired = (int)Math.Ceiling(_mergeAreas.Count / (double)areasPerRecord);
            for (int recordIndex = 0; recordIndex < recordsRequired; recordIndex++)
            {
                ushort blockAreaIndex = 0;
                Bytes rangeAddresses = new Bytes();
                while (areaIndex < mergeAreaCount && blockAreaIndex < areasPerRecord)
                {
                    rangeAddresses.Append(CellRangeAddress(_mergeAreas[areaIndex]));

                    blockAreaIndex++;
                    areaIndex++;
                }
                rangeAddresses.Prepend(BitConverter.GetBytes(blockAreaIndex));
                mergedcells.Append(Record.GetBytes(RID.MERGEDCELLS, rangeAddresses));
            }

            return mergedcells;
        }

        private Bytes CellRangeAddress(MergeArea mergeArea)
        {
            return CellRangeAddress(mergeArea.RowMin, mergeArea.RowMax, mergeArea.ColMin, mergeArea.ColMax);
        }

        private Bytes CellRangeAddress(ushort minRow, ushort maxRow, ushort minCol, ushort maxCol)
        {
            minRow--;
            maxRow--;
            minCol--;
            maxCol--;
            Bytes rangeAddress = new Bytes();
            rangeAddress.Append(BitConverter.GetBytes(minRow));
            rangeAddress.Append(BitConverter.GetBytes(maxRow));
            rangeAddress.Append(BitConverter.GetBytes(minCol));
            rangeAddress.Append(BitConverter.GetBytes(maxCol));
            return rangeAddress;
        }

        //TODO: I think this should actually be MRBlockRow --- see use in RowBlocks.BlockRow
        internal CachedBlockRow CachedBlockRow
        {
            get { return _cachedBlockRow; }
            set { _cachedBlockRow = value; }
        }

        /// <summary>
        /// Adds a Column Info record to this Worksheet.
        /// </summary>
        /// <param name="columnInfo">The ColumnInfo object to add to this Worksheet.</param>
        public void AddColumnInfo(ColumnInfo columnInfo)
        {
            //TODO: Implement existence checking & deletion / overwriting / not-adding
            //NOTE: Don't know if this is necessary (i.e. does Excel allow "adding" values of overlapping ColInfos?
            _columnInfos.Add(columnInfo);
        }

        //TODO: Optionally provide overload with bool parameter to decide whether to throw
        //exception instead of losing values.
        /// <summary>
        /// Adds a MergeArea to this Worksheet.  The mergeArea is verified not to
        /// overlap with any previously defined area.  NOTE Values and formatting
        /// in all cells other than the first in mergeArea (scanning left to right,
        /// top to bottom) will be lost.
        /// </summary>
        /// <param name="mergeArea">The MergeArea to add to this Worksheet.</param>
        public void AddMergeArea(MergeArea mergeArea)
        {
            foreach (MergeArea existingArea in _mergeAreas)
            {
                bool colsOverlap = false;
                bool rowsOverlap = false;

                //if they overlap, either mergeArea will surround existingArea, 
                if (mergeArea.ColMin < existingArea.ColMin && existingArea.ColMax < mergeArea.ColMax)
                    colsOverlap = true;
                //or existingArea will contain >= 1 of mergeArea's Min and Max indices
                else if ((existingArea.ColMin <= mergeArea.ColMin && existingArea.ColMax >= mergeArea.ColMin) ||
                    (existingArea.ColMin <= mergeArea.ColMax && existingArea.ColMax >= mergeArea.ColMax))
                    colsOverlap = true;

                if (mergeArea.RowMin < existingArea.RowMin && existingArea.RowMax < mergeArea.RowMax)
                    rowsOverlap = true;
                else if ((existingArea.RowMin <= mergeArea.RowMin && existingArea.RowMax >= mergeArea.RowMin) ||
                    (existingArea.RowMin <= mergeArea.RowMax && existingArea.RowMax >= mergeArea.RowMax))
                    rowsOverlap = true;

                if (colsOverlap && rowsOverlap)
                    throw new ArgumentException("overlaps with existing MergeArea", "mergeArea");
            }

            //TODO: Add ref to this mergeArea to all rows in its range, and add checking on Cell
            //addition methods to validate they are not being added within the mergedarea, other
            //than as the top-left cell.

            _mergeAreas.Add(mergeArea);
        }

        /// <summary>
        /// Writes a DataTable to this Worksheet, beginning at the provided Row
        /// and Column indices.  A Header Row will be written.
        /// </summary>
        /// <param name="table">The DataTable to write to this Worksheet.</param>
        /// <param name="startRow">The Row at which to start writing the DataTable
        /// to this Worksheet (1-based).</param>
        /// <param name="startCol">The Column at which to start writing the DataTable
        /// to this Worksheet (1-based).</param>
        public void Write(DataTable table, int startRow, int startCol)
        {
            if ((table.Columns.Count + startCol) > BIFF8.MaxCols)
                throw new ApplicationException(string.Format("Table {0} has too many columns {1} to fit on Worksheet {2} with the given startCol {3}",
                                               table.TableName, table.Columns.Count, BIFF8.MaxCols, startCol));
            if ((table.Rows.Count + startRow) > (BIFF8.MaxRows - 1))
                throw new ApplicationException(string.Format("Table {0} has too many rows {1} to fit on Worksheet {2} with the given startRow {3}",
                                               table.TableName, table.Rows.Count, (BIFF8.MaxRows - 1), startRow));
            int row = startRow;
            int col = startCol;
            foreach (DataColumn dataColumn in table.Columns)
                Cells.Add(row, col++, dataColumn.ColumnName);
            foreach (DataRow dataRow in table.Rows)
            {
                row++;
                col = startCol;
                foreach (object dataItem in dataRow.ItemArray)
                {
                    object value = dataItem;

                    if (dataItem == DBNull.Value)
                        value = null;
                    if (dataRow.Table.Columns[col - startCol].DataType == typeof(byte[]))
                        value = string.Format("[ByteArray({0})]", ((byte[])value).Length);

                    Cells.Add(row, col++, value);
                }
            }
        }
    }

}
namespace Core.MyXls
{
    /// <summary>
    /// Manages the collection of Worksheets for a Workbook.
    /// </summary>
    public class Worksheets : IEnumerable<Worksheet>
    {
        private List<Worksheet> _worksheets = new List<Worksheet>();

        private readonly XlsDocument _doc;

        private int _streamOffset;

        internal Worksheets(XlsDocument doc)
            : base()
        {
            _doc = doc;
        }

        internal void Add(Record boundSheetRecord, List<Record> sheetRecords)
        {
            Worksheet sheet = new Worksheet(_doc, boundSheetRecord, sheetRecords);
            _worksheets.Add(sheet);
        }

        /// <summary>
        /// Adds a Worksheet with the given name to this collection.
        /// </summary>
        /// <param name="name">The name of the new worksheet.</param>
        /// <returns>The new Worksheet with the given name in this collection.</returns>
        public Worksheet Add(string name)
        {
            Worksheet sheet = new Worksheet(_doc);
            sheet.Name = name;
            _worksheets.Add(sheet);
            return sheet;
        }

        /// <summary>
        /// Gets the count of Worksheet objects in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _worksheets.Count;
            }
        }

        /// <summary>
        /// OBSOLETE - Use Add(string) instead.  Adds a Worksheet with the given 
        /// name to this collection.
        /// </summary>
        /// <param name="name">The name of the new worksheet.</param>
        /// <returns>The new Worksheet with the given name in this collection.</returns>
        [Obsolete]
        public Worksheet AddNamed(string name)
        {
            return Add(name);
        }

        /// <summary>
        /// Gets the Worksheet from this collection with the given index.
        /// </summary>
        /// <param name="index">The index of the Worksheet in this collection to get.</param>
        /// <returns>The Worksheet from this collection with the given index.</returns>
        public Worksheet this[int index]
        {
            get
            {
                return _worksheets[index];
            }
        }

        /// <summary>
        /// Gets the Worksheet from this collection with the given name.
        /// </summary>
        /// <param name="name">The name of the Worksheet in this collection to get.</param>
        /// <returns>The Worksheet from this collection with the given name.</returns>
        public Worksheet this[string name]
        {
            get
            {
                return this[GetIndex(name)];
            }
        }

        /// <summary>
        /// Gets the index of the Workseet in this collection by the given name.
        /// </summary>
        /// <param name="sheetName">The name of the Worksheet for which to return the index.</param>
        /// <returns>The index of the Worksheet by the given name.</returns>
        public int GetIndex(string sheetName)
        {
            int i = 0;
            foreach (Worksheet sheet in this)
            {
                if (string.Compare(sheet.Name, sheetName, false) == 0)
                    return i;
                i++;
            }

            throw new IndexOutOfRangeException(sheetName);
        }

        internal int StreamOffset
        {
            get { return _streamOffset; }
            set { _streamOffset = value; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of Worksheets.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection of worksheets.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Worksheet> GetEnumerator()
        {
            return new WorksheetEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection of worksheets.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ///<summary>
        /// Enumerator for the Workseets collection.
        ///</summary>
        public class WorksheetEnumerator : IEnumerator<Worksheet>
        {
            private Worksheets _worksheets = null;
            private int _index = -1;

            ///<summary>
            /// Creates and initializes a new instance of the WorksheetEnumerator class for the given Worksheets collection instance.
            ///</summary>
            ///<param name="worksheets">The Worksheets object for which to initialize this WorksheetEnumerator object.</param>
            ///<exception cref="ArgumentNullException"></exception>
            public WorksheetEnumerator(Worksheets worksheets)
            {
                if (worksheets == null)
                    throw new ArgumentNullException("worksheets");

                _worksheets = worksheets;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                //no-op (no resources to release)
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
            public bool MoveNext()
            {
                if (_worksheets.Count == 0)
                    return false;

                if (_index == (_worksheets.Count - 1))
                    return false;

                _index++;
                return true;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
            public void Reset()
            {
                _index = -1;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public Worksheet Current
            {
                get { return _worksheets[_index]; }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <returns>
            /// The current element in the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception><filterpriority>2</filterpriority>
            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Excel Worksheet Types
    /// </summary>
    public enum WorksheetTypes
    {
        /// <summary>Default (Worksheet)</summary>
        Default = Worksheet,
        /// <summary>Worksheet (Default)</summary>
        Worksheet = 1,
        /// <summary>Chart Worksheet</summary>
        Chart = 2,
        /// <summary>VB Module Worksheet</summary>
        VBModule = 3
    }

    internal static class WorksheetType
    {
        internal static byte[] GetBytes(WorksheetTypes type)
        {
            switch (type)
            {
                case WorksheetTypes.Worksheet: return new byte[] { 0x00 };
                case WorksheetTypes.Chart: return new byte[] { 0x02 };
                case WorksheetTypes.VBModule: return new byte[] { 0x04 };
                default: throw new ApplicationException(string.Format("Unexpected WorksheetTypes {0}", type));
            }
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Worksheet Visibility values available in Excel (whether the Worksheet will be visible to the user).
    /// </summary>
    public enum WorksheetVisibilities
    {
        /// <summary>Default - Visible</summary>
        Default = Visible,

        /// <summary>Visible</summary>
        Visible = 1,

        /// <summary>Hidden</summary>
        Hidden = 2,

        /// <summary>Strong Hidden (used for VBA modules)</summary>
        StrongHidden = 3,
    }

    internal static class WorksheetVisibility
    {
        internal static byte[] GetBytes(WorksheetVisibilities visibility)
        {
            switch (visibility)
            {
                case WorksheetVisibilities.Visible: return new byte[] { 0x00 };
                case WorksheetVisibilities.Hidden: return new byte[] { 0x01 };
                case WorksheetVisibilities.StrongHidden: return new byte[] { 0x02 };
                default: throw new ApplicationException(string.Format("Unexpected WorksheetVisibilities {0}", visibility));
            }
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// The XF (eXtended Format) contains formatting information for cells, rows, columns or styles.
    /// </summary>
    public class XF : ICloneable
    {
        private IXFTarget _targetObject;

        private readonly XlsDocument _doc;
        private ushort? _id;

        internal ushort? ReadStyleXfIndex = null;

        //        private const ushort DEFAULT_LINE_COLOUR_INDEX = 8;

        internal XF(XlsDocument doc)
        {
            _doc = doc;

            _id = null;

            SetDefaults();
        }

        internal XF(XlsDocument doc, Bytes bytes, Font font, string format)
            : this(doc)
        {
            ReadBytes(bytes, font, format);
        }

        internal IXFTarget Target
        {
            get { return _targetObject; }
            set
            {
                _targetObject = value;
                _font.Target = this;
            }
        }

        internal void OnFontChange(Font newFont)
        {
            _font = (Font)newFont.Clone();
            //_font.ID = newFont.ID;
            OnChange();
        }

        internal void OnChange()
        {
            _id = null;
            if (_targetObject != null)
                _targetObject.UpdateId(this);
        }

        private void SetDefaults()
        {
            _font = new Font(_doc, this);
            _format = Formats.Default;
            _style = new Style(_doc, this);

            _horizontalAlignment = HorizontalAlignments.Default;
            _textWrapRight = false;
            _verticalAlignment = VerticalAlignments.Default;
            _rotation = 0;
            _indentLevel = 0;
            _shrinkToCell = false;
            _textDirection = TextDirections.Default;
            _cellLocked = false; //NOTE: Unsure about this default (compare to Commented XF String in BinData)
            _formulaHidden = false; //NOTE: Unsure about this default (compare to Commented XF String in BinData)
            _isStyleXF = false; //NOTE: Unsure about this default (compare to Commented XF String in BinData)
            _useNumber = true;
            _useFont = true;
            _useMisc = true;
            _useBorder = true;
            _useBackground = true;
            _useProtection = true; //You should ALWAYS use protection ;-)
            _leftLineStyle = 0;
            _rightLineStyle = 0;
            _topLineStyle = 0;
            _bottomLineStyle = 0;
            _leftLineColor = Colors.DefaultLineColor;
            _rightLineColor = Colors.DefaultLineColor;
            _diagonalDescending = false;
            _diagonalAscending = false;
            _topLineColor = Colors.DefaultLineColor;
            _bottomLineColor = Colors.DefaultLineColor;
            _diagonalLineColor = Colors.DefaultLineColor;
            _diagonalLineStyle = LineStyle.None;
            _pattern = 0;
            _patternColor = Colors.DefaultPatternColor;
            _patternBackgroundColor = Colors.DefaultPatternBackgroundColor;

            OnChange();
        }

        private Font _font;
        private string _format = Formats.Default;
        private Style _style;

        /// <summary>
        /// Gets or sets the Font for this XF.
        /// </summary>
        public Font Font
        {
            get
            {
                if (_font == null)
                    _font = new Font(_doc, this);

                return _font;
            }
            set
            {
                if (_font == null && value == null)
                    return;
                else if (_font != null && value != null && value.Equals(_font))
                    return;

                _font = (value == null ? null : (Font)value.Clone());
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets the Format for this XF.
        /// </summary>
        public string Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (value == null)
                    value = Formats.Default;

                if (value.Length > 65535)
                    value = value.Substring(0, 65535);

                if (string.Compare(value, _format, false) == 0)
                    return;

                _format = (string)value.Clone();
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets the Parent Style XF (absent on Style XF's).
        /// </summary>
        public Style Style
        {
            get
            {
                if (_style == null)
                    _style = new Style(_doc, this);
                return _style;
            }
            set
            {
                if (_style == null && value == null)
                    return;
                else if (_style != null && value != null && value.Equals(_style))
                    return;

                _style = (value == null ? null : (Style)value.Clone());
                OnChange();
            }
        }

        private HorizontalAlignments _horizontalAlignment;
        private bool _textWrapRight;
        private VerticalAlignments _verticalAlignment;
        private short _rotation;
        private ushort _indentLevel;
        private bool _shrinkToCell;
        private TextDirections _textDirection;

        /// <summary>
        /// Gets or sets the HorizontalAlignments value for this XF.
        /// </summary>
        public HorizontalAlignments HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                if (value == _horizontalAlignment)
                    return;

                _horizontalAlignment = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets whether Text is wrapped at right border.
        /// </summary>
        public bool TextWrapRight
        {
            get { return _textWrapRight; }
            set
            {
                if (value == _textWrapRight)
                    return;

                _textWrapRight = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets the VerticalAlignments value for this XF.
        /// </summary>
        public VerticalAlignments VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                if (value == _verticalAlignment)
                    return;

                _verticalAlignment = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets the Text rotation angle for this XF (-360 or 0?) to 360.
        /// </summary>
        public short Rotation
        {
            get { return _rotation; }
            set
            {
                if (value == _rotation)
                    return;

                //NOTE: This looks wrong!
                if (value < 0)
                    value = 0;
                if (value > 180 && value != 255)
                    value = 0;
                _rotation = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets the IndentLevel for this XF.
        /// </summary>
        public ushort IndentLevel
        {
            get { return _indentLevel; }
            set
            {
                if (value == _indentLevel)
                    return;

                _indentLevel = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets whether to Shrink content to fit into cell for this XF.
        /// </summary>
        public bool ShrinkToCell
        {
            get { return _shrinkToCell; }
            set
            {
                if (value == _shrinkToCell)
                    return;

                _shrinkToCell = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets the TextDirections value for this XF (BIFF8X only).
        /// </summary>
        public TextDirections TextDirection
        {
            get { return _textDirection; }
            set
            {
                if (value == _textDirection)
                    return;

                _textDirection = value;
                OnChange();
            }
        }

        #region XF_TYP_PROT : XF Type, Cell Protection

        private bool _cellLocked;
        private bool _formulaHidden;
        private bool _isStyleXF;

        /// <summary>
        /// Gets or sets whether this XF's Cells are locked.
        /// </summary>
        public bool CellLocked
        {
            get { return _cellLocked; }
            set
            {
                if (value == _cellLocked)
                    return;

                _cellLocked = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets whether this XF's Formulas are hidden
        /// </summary>
        public bool FormulaHidden
        {
            get { return _formulaHidden; }
            set
            {
                if (value == _formulaHidden)
                    return;

                _formulaHidden = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets whether this XF is a Style XF.
        /// </summary>
        internal bool IsStyleXF
        {
            get { return _isStyleXF; }
            set
            {
                if (value == _isStyleXF)
                    return;

                _isStyleXF = value;

                OnChange();
            }
        }

        #endregion

        #region XF_USED_ATTRIB : Used Attributes
        //--------------------------------------------------------------------------------
        //Each bit describes the validity of a specific group of attributes. In cell XFs a 
        //cleared bit means the attributes of the parent style XF are used (but only if the 
        //attributes are valid there), a set bit means the attributes of this XF are used. 
        //In style XFs a cleared bit means the attribute setting is valid, a set bit means 
        //the attribute should be ignored.
        //                      - excelfileformat.pdf, section 6.115.1 under XF_USED_ATTRIB
        //--------------------------------------------------------------------------------
        //In this implementation True -> Set and False -> Cleared
        //NOTE: Is this how Excel implements Paste w(/o) Formats, Fonts, Borders, etc?
        //NOTE: Is this how Excel implements its Format Painter?

        private bool _useNumber;
        private bool _useFont;
        private bool _useMisc;
        private bool _useBorder;
        private bool _useBackground;
        private bool _useProtection;

        /// <summary>
        /// Gets or sets Flag for number format.
        /// </summary>
        public bool UseNumber
        {
            get { return _useNumber; }
            set
            {
                if (value == _useNumber)
                    return;

                _useNumber = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Flag for font.
        /// </summary>
        public bool UseFont
        {
            get { return _useFont; }
            set
            {
                if (value == _useFont)
                    return;

                _useFont = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Flag for horizontal and vertical alignment, text wrap, indentation, 
        /// orientation, rotation, and text direction.
        /// </summary>
        public bool UseMisc
        {
            get { return _useMisc; }
            set
            {
                if (value == _useMisc)
                    return;

                _useMisc = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Flag for border lines.
        /// </summary>
        public bool UseBorder
        {
            get { return _useBorder; }
            set
            {
                if (value == _useBorder)
                    return;

                _useBorder = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Flag for background area style.
        /// </summary>
        public bool UseBackground
        {
            get { return _useBackground; }
            set
            {
                if (value == _useBackground)
                    return;

                _useBackground = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Flag for cell protection (cell locked and formula hidden).
        /// </summary>
        public bool UseProtection
        {
            get { return _useProtection; }
            set
            {
                if (value == _useProtection)
                    return;

                _useProtection = value;
                OnChange();
            }
        }

        #endregion

        #region XF_BORDER_LINES_BG : Border lines, background

        private ushort _leftLineStyle;
        private ushort _rightLineStyle;
        private ushort _topLineStyle;
        private ushort _bottomLineStyle;
        private Color _leftLineColor;
        private Color _rightLineColor;
        private bool _diagonalDescending;
        private bool _diagonalAscending;

        /// <summary>
        /// Gets or sets Left line style.
        /// </summary>
        public ushort LeftLineStyle
        {
            get { return _leftLineStyle; }
            set
            {
                if (value == _leftLineStyle)
                    return;

                _leftLineStyle = value;

                //If LineStyle is set (!=0) and LineColour is not set (==0),
                //the Cell Formatting menu will fail to open, though the 
                //formatting will be displayed.  Setting LineColour without a
                //LineStyle does not appear to cause a problem.  This is true
                //with Left, Right, Top, Bottom, DiagonalAscending and
                //DiagonalDescending lines.
                //                if (_leftLineStyle == 0)
                //                    _leftLineColor = Colors.DefaultLineColo;
                //                else if (_leftLineColorIndex == 0)
                //                    _leftLineColorIndex = DEFAULT_LINE_COLOUR_INDEX;

                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Right line style.
        /// </summary>
        public ushort RightLineStyle
        {
            get { return _rightLineStyle; }
            set
            {
                if (value == _rightLineStyle)
                    return;

                _rightLineStyle = value;

                //If LineStyle is set (!=0) and LineColour is not set (==0),
                //the Cell Formatting menu will fail to open, though the 
                //formatting will be displayed.  Setting LineColour without a
                //LineStyle does not appear to cause a problem.  This is true
                //with Left, Right, Top, Bottom, DiagonalAscending and
                //DiagonalDescending lines.
                //                if (_rightLineStyle == 0)
                //                    _rightLineColorIndex = 0;
                //                else if (_rightLineColorIndex == 0)
                //                    _rightLineColorIndex = DEFAULT_LINE_COLOUR_INDEX;

                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Top line style.
        /// </summary>
        public ushort TopLineStyle
        {
            get { return _topLineStyle; }
            set
            {
                if (value == _topLineStyle)
                    return;

                _topLineStyle = value;

                //If LineStyle is set (!=0) and LineColour is not set (==0),
                //the Cell Formatting menu will fail to open, though the 
                //formatting will be displayed.  Setting LineColour without a
                //LineStyle does not appear to cause a problem.  This is true
                //with Left, Right, Top, Bottom, DiagonalAscending and
                //DiagonalDescending lines.
                //                if (_topLineStyle == 0)
                //                    _topLineColorIndex = 0;
                //                else if (_topLineColorIndex == 0)
                //                    _topLineColorIndex = DEFAULT_LINE_COLOUR_INDEX;

                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Bottom line style.
        /// </summary>
        public ushort BottomLineStyle
        {
            get { return _bottomLineStyle; }
            set
            {
                if (value == _bottomLineStyle)
                    return;

                _bottomLineStyle = value;

                //If LineStyle is set (!=0) and LineColour is not set (==0),
                //the Cell Formatting menu will fail to open, though the 
                //formatting will be displayed.  Setting LineColour without a
                //LineStyle does not appear to cause a problem.  This is true
                //with Left, Right, Top, Bottom, DiagonalAscending and
                //DiagonalDescending lines.
                //                if (_bottomLineStyle == 0)
                //                    _bottomLineColorIndex = 0;
                //                else if (_bottomLineColorIndex == 0)
                //                    _bottomLineColorIndex = DEFAULT_LINE_COLOUR_INDEX;

                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Colour for left line.
        /// </summary>
        public Color LeftLineColor
        {
            get { return _leftLineColor; }
            set
            {
                if (value.Equals(_leftLineColor))
                    return;

                _leftLineColor = (Color)value.Clone();
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Colour for right line.
        /// </summary>
        public Color RightLineColor
        {
            get { return _rightLineColor; }
            set
            {
                if (value.Equals(_rightLineColor))
                    return;

                _rightLineColor = (Color)value.Clone();
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Diagonal line from top left to right bottom.
        /// </summary>
        public bool DiagonalDescending
        {
            get { return _diagonalDescending; }
            set
            {
                if (value == _diagonalDescending)
                    return;

                _diagonalDescending = value;
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Diagonal line from bottom left to top right.  Lines won't show up unless
        /// DiagonalLineStyle (and color - defaults to black, though) is set.
        /// </summary>
        public bool DiagonalAscending
        {
            get { return _diagonalAscending; }
            set
            {
                if (value == _diagonalAscending)
                    return;

                _diagonalAscending = value;
                OnChange();
            }
        }

        #endregion

        #region XF_LINE_COLOUR_STYLE_FILL : Line colour, style, and fill

        private Color _topLineColor;
        private Color _bottomLineColor;
        private Color _diagonalLineColor;
        private LineStyle _diagonalLineStyle;
        private ushort _pattern;

        /// <summary>
        /// Gets or sets Colour for top line.
        /// </summary>
        public Color TopLineColor
        {
            get { return _topLineColor; }
            set
            {
                if (value.Equals(_topLineColor))
                    return;

                _topLineColor = (Color)value.Clone();
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Colour for bottom line.
        /// </summary>
        public Color BottomLineColor
        {
            get { return _bottomLineColor; }
            set
            {
                if (value.Equals(_bottomLineColor))
                    return;

                _bottomLineColor = (Color)value.Clone();
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Colour for diagonal line.
        /// </summary>
        public Color DiagonalLineColor
        {
            get { return _diagonalLineColor; }
            set
            {
                if (value.Equals(_diagonalLineColor))
                    return;

                _diagonalLineColor = (Color)value.Clone();
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Diagonal line style.
        /// </summary>
        public LineStyle DiagonalLineStyle
        {
            get { return _diagonalLineStyle; }
            set
            {
                if (value == _diagonalLineStyle)
                    return;

                _diagonalLineStyle = value;

                //If LineStyle is set (!=0) and LineColour is not set (==0),
                //the Cell Formatting menu will fail to open, though the 
                //formatting will be displayed.  Setting LineColour without a
                //LineStyle does not appear to cause a problem.  This is true
                //with Left, Right, Top, Bottom, DiagonalAscending and
                //DiagonalDescending lines.
                //                if (_diagonalLineStyle == 0)
                //                    _diagonalLineColorIndex = 0;
                //                else if (_diagonalLineColorIndex == 0)
                //                    _diagonalLineColorIndex = DEFAULT_LINE_COLOUR_INDEX;

                OnChange();
            }
        }

        //TODO: Create Standard Fill Pattern constants
        /// <summary>
        /// Gets or sets Fill pattern.
        /// </summary>
        public ushort Pattern
        {
            get { return _pattern; }
            set
            {
                if (value == _pattern)
                    return;

                _pattern = value;
                OnChange();
            }
        }

        #endregion

        #region XF_PATTERN : Pattern

        private Color _patternColor;
        private Color _patternBackgroundColor;

        /// <summary>
        /// Gets or sets Colour for pattern colour.
        /// </summary>
        public Color PatternColor
        {
            get { return _patternColor; }
            set
            {
                if (value.Equals(_patternColor))
                    return;

                _patternColor = (Color)value.Clone();
                OnChange();
            }
        }

        /// <summary>
        /// Gets or sets Colour for pattern background.
        /// </summary>
        public Color PatternBackgroundColor
        {
            get { return _patternBackgroundColor; }
            set
            {
                if (value.Equals(_patternBackgroundColor))
                    return;

                _patternBackgroundColor = (Color)value.Clone();
                OnChange();
            }
        }

        #endregion

        private void ReadBytes(Bytes bytes, Font font, string format)
        {
            _font = font;
            _format = format;
            ReadXF_3(bytes.Get(4, 2));
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                bytes.Append(BitConverter.GetBytes(_font.ID));
                bytes.Append(BitConverter.GetBytes(_doc.Workbook.Formats.GetFinalID(_format)));
                bytes.Append(XF_3());
                bytes.Append(XF_ALIGN());
                bytes.Append((byte)_rotation);
                bytes.Append(XF_6());
                bytes.Append(XF_USED_ATTRIB());
                bytes.Append(XF_BORDER_LINES_BG());
                bytes.Append(XF_LINE_COLOUR_STYLE_FILL());
                bytes.Append(XF_PATTERN());

                return Record.GetBytes(RID.XF, bytes);
            }
        }

        private void ReadXF_3(Bytes bytes)
        {
            Bytes.Bits bits = bytes.GetBits();

            ushort parentStyleXfIndex = bits.Get(4, 12).ToUInt16();
            if (parentStyleXfIndex == 4095)
            {
                //this is a Style XF -- do nothing
            }
            else
            {
                ReadStyleXfIndex = parentStyleXfIndex; //we'll assign the style xf index later using the xfIdxLookups collected by Workbook.ReadBytes()
            }

            ReadXF_TYPE_PROT(bits.Get(4));
        }

        private byte[] XF_3()
        {
            ushort value, styleXfIdx;

            if (_isStyleXF)
                styleXfIdx = 4095;
            else
                //NOTE: This forces all cell (non-Style) XF's to use Style XF with index 0;
                styleXfIdx = 0; // TODO : Change this to implement Style XF's

            value = 0;
            value += XF_TYP_PROT();
            value += (ushort)(styleXfIdx * 16);
            return BitConverter.GetBytes(value);
        }

        private void ReadXF_TYPE_PROT(Bytes.Bits bits)
        {

        }

        private ushort XF_TYP_PROT()
        {
            ushort value = 0;
            if (_cellLocked)
                value += 1;
            if (_formulaHidden)
                value += 2;
            if (_isStyleXF)
                value += 4;
            return value;
        }

        private byte XF_ALIGN()
        {
            byte value = 0;
            value += (byte)((byte)_verticalAlignment * 16);
            if (_textWrapRight)
                value += 8;
            value += (byte)_horizontalAlignment;
            return value;
        }

        private byte XF_6()
        {
            ushort value = 0;
            value += (ushort)((ushort)_textDirection * 64);
            if (_shrinkToCell)
                value += 16;
            value += _indentLevel;
            return (byte)value;
        }

        private byte XF_USED_ATTRIB()
        {
            ushort value = 0;
            if (_isStyleXF ? !_useNumber : _useNumber)
                value += 1;
            if (_isStyleXF ? !_useFont : _useFont)
                value += 2;
            if (_isStyleXF ? !_useMisc : _useMisc)
                value += 4;
            if (_isStyleXF ? !_useBorder : _useBorder)
                value += 8;
            if (_isStyleXF ? !_useBackground : _useBackground)
                value += 16;
            if (_isStyleXF ? !_useProtection : _useProtection)
                value += 32;
            value *= 4;
            return (byte)value;
        }

        //OPTIM: Use bit-shifting instead of Math.Pow
        private byte[] XF_BORDER_LINES_BG()
        {
            uint value = 0;
            value += _leftLineStyle;
            value += (uint)(Math.Pow(2, 4) * _rightLineStyle);
            value += (uint)(Math.Pow(2, 8) * _topLineStyle);
            value += (uint)(Math.Pow(2, 12) * _bottomLineStyle);
            value += (uint)(Math.Pow(2, 16) * _doc.Workbook.Palette.GetIndex(_leftLineColor));
            value += (uint)(Math.Pow(2, 23) * _doc.Workbook.Palette.GetIndex(_rightLineColor));
            if (_diagonalDescending)
                value += (uint)Math.Pow(2, 30);
            if (_diagonalAscending)
                value += (uint)Math.Pow(2, 31);
            return BitConverter.GetBytes(value);
        }

        //OPTIM: Use bit-shifting instead of Math.Pow
        private byte[] XF_LINE_COLOUR_STYLE_FILL()
        {
            uint value = 0;
            value += _doc.Workbook.Palette.GetIndex(_topLineColor);
            value += (uint)(Math.Pow(2, 7) * _doc.Workbook.Palette.GetIndex(_bottomLineColor));
            value += (uint)(Math.Pow(2, 14) * _doc.Workbook.Palette.GetIndex(_diagonalLineColor));
            value += (uint)(Math.Pow(2, 21) * (ushort)_diagonalLineStyle);
            value += (uint)(Math.Pow(2, 26) * _pattern);
            return BitConverter.GetBytes(value);
        }

        private byte[] XF_PATTERN()
        {
            ushort value = 0;
            value += _doc.Workbook.Palette.GetIndex(_patternColor);
            value += (ushort)(Math.Pow(2, 7) * _doc.Workbook.Palette.GetIndex(_patternBackgroundColor));
            return BitConverter.GetBytes(value);
        }

        internal bool Equals(XF that)
        {
            if (_horizontalAlignment != that._horizontalAlignment) return false;
            if (_textWrapRight != that._textWrapRight) return false;
            if (_verticalAlignment != that._verticalAlignment) return false;
            if (_rotation != that._rotation) return false;
            if (_indentLevel != that._indentLevel) return false;
            if (_shrinkToCell != that._shrinkToCell) return false;
            if (_textDirection != that._textDirection) return false;
            if (_cellLocked != that._cellLocked) return false;
            if (_formulaHidden != that._formulaHidden) return false;
            if (_isStyleXF != that._isStyleXF) return false;
            if (_useNumber != that._useNumber) return false;
            if (_useFont != that._useFont) return false;
            if (_useMisc != that._useMisc) return false;
            if (_useBorder != that._useBorder) return false;
            if (_useBackground != that._useBackground) return false;
            if (_useProtection != that._useProtection) return false;
            if (_leftLineStyle != that._leftLineStyle) return false;
            if (_rightLineStyle != that._rightLineStyle) return false;
            if (_topLineStyle != that._topLineStyle) return false;
            if (_bottomLineStyle != that._bottomLineStyle) return false;
            if (!_leftLineColor.Equals(that._leftLineColor)) return false;
            if (!_rightLineColor.Equals(that._rightLineColor)) return false;
            if (_diagonalDescending != that._diagonalDescending) return false;
            if (_diagonalAscending != that._diagonalAscending) return false;
            if (!_topLineColor.Equals(that._topLineColor)) return false;
            if (!_bottomLineColor.Equals(that._bottomLineColor)) return false;
            if (!_diagonalLineColor.Equals(that._diagonalLineColor)) return false;
            if (_diagonalLineStyle != that._diagonalLineStyle) return false;
            if (_pattern != that._pattern) return false;
            if (!_patternColor.Equals(that._patternColor)) return false;
            if (!_patternBackgroundColor.Equals(that._patternBackgroundColor)) return false;

            if (!Font.Equals(that.Font)) return false;
            if (!Format.Equals(that.Format)) return false;
            if (!Style.Equals(that.Style)) return false;

            //if (_targetObject != that._targetObject) return false;

            return true;
        }

        internal ushort Id
        {
            get
            {
                if (_id == null)
                    _id = _doc.Workbook.XFs.Add(this);
                return (ushort)_id;
            }
        }

        #region ICloneable members

        /// <summary>
        /// Creates a duplicate instance of this XF objet.
        /// </summary>
        /// <returns>A duplicate instance of this XF object.</returns>
        public object Clone()
        {
            XF clone = new XF(_doc);

            clone.Font = (Font)_font.Clone();
            clone.Format = (string)_format.Clone();

            if (!IsStyleXF)
                clone.Style = (Style)_style.Clone();

            clone.HorizontalAlignment = HorizontalAlignment;
            clone.TextWrapRight = TextWrapRight;
            clone.VerticalAlignment = VerticalAlignment;
            clone.Rotation = Rotation;
            clone.IndentLevel = IndentLevel;
            clone.ShrinkToCell = ShrinkToCell;
            clone.TextDirection = TextDirection;
            clone.CellLocked = CellLocked;
            clone.FormulaHidden = FormulaHidden;
            clone.IsStyleXF = IsStyleXF;
            clone.UseNumber = UseNumber;
            clone.UseFont = UseFont;
            clone.UseMisc = UseMisc;
            clone.UseBorder = UseBorder;
            clone.UseBackground = UseBackground;
            clone.UseProtection = UseProtection;
            clone.LeftLineStyle = LeftLineStyle;
            clone.RightLineStyle = RightLineStyle;
            clone.TopLineStyle = TopLineStyle;
            clone.BottomLineStyle = BottomLineStyle;
            clone.LeftLineColor = LeftLineColor;
            clone.RightLineColor = RightLineColor;
            clone.DiagonalDescending = DiagonalDescending;
            clone.DiagonalAscending = DiagonalAscending;
            clone.TopLineColor = TopLineColor;
            clone.BottomLineColor = BottomLineColor;
            clone.DiagonalLineColor = DiagonalLineColor;
            clone.DiagonalLineStyle = DiagonalLineStyle;
            clone.Pattern = Pattern;
            clone.PatternColor = PatternColor;
            clone.PatternBackgroundColor = PatternBackgroundColor;

            clone.Target = Target;

            return clone;
        }

        #endregion
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// Manages the collection of XF objects for a Workbook.
    /// </summary>
    public class XFs
    {
        private readonly XlsDocument _doc;
        private readonly Workbook _workbook;

        private XF _defaultUserXf;

        private readonly List<XF> _xfs;

        internal SortedList<ushort, ushort> XfIdxLookups = null;

        internal XFs(XlsDocument doc, Workbook workbook)
        {
            _doc = doc;
            _workbook = workbook;

            _xfs = new List<XF>();

            AddDefaultStyleXFs();
            AddDefaultUserXF();
            //AddDefaultFormattedStyleXFs();  //what was I thinking about here?
        }

        internal XF this[int index]
        {
            get { return (XF)_xfs[index].Clone(); }
        }

        internal XF DefaultUserXF
        {
            get { return (XF)_defaultUserXf.Clone(); }
        }

        internal ushort Add(XF xf)
        {
            _workbook.Fonts.Add(xf.Font);
            _workbook.Formats.Add(xf.Format);
            _workbook.Styles.Add(xf.Style);

            //TODO: What happens if they try to re-add a Default (i.e. non-user) XF?
            short xfId = GetId(xf);
            if (xfId == -1)
            {
                xfId = (short)_xfs.Count;
                _xfs.Add((XF)xf.Clone());
            }

            //NOTE: Not documented, but User-defined XFs must have a minimum
            //index of 16 (0-based).

            return (ushort)xfId;
        }

        private void AddDefaultStyleXFs()
        {
            XF xf = new XF(_doc);
            xf.IsStyleXF = true;
            xf.CellLocked = true; //TODO: Is this correct?  Default Style XF is CellLocked?  what's the origin of this line?
            _xfs.Add(xf);

            xf = (XF)xf.Clone();
            xf.UseBackground = false;
            xf.UseBorder = false;
            xf.UseFont = true;
            xf.UseMisc = false;
            xf.UseNumber = false;
            xf.UseProtection = false;

            //Gotta have a 16th (index 15) XF for the Default Cell Format...
            //See excelfileformat.pdf Sec. 4.6.2: The default cell format is always present
            //in an Excel file, described by the XF record with the fixed index 15 (0-based).
            //By default, it uses the worksheet/workbook default cell style, described by
            //the very first XF record (index 0);
            //Apparently Excel 2003 was okay without it, but 2007 chokes if it's not there.
            for (int i = 0; i < 15; i++)
                _xfs.Add((XF)xf.Clone());
        }

        private void AddDefaultUserXF()
        {
            XF xf = new XF(_doc);
            xf.CellLocked = true;

            Add(xf);

            _defaultUserXf = xf;
        }

        //        private void AddDefaultFormattedStyleXFs()
        //        {
        //            
        //        }

        private short GetId(XF xf)
        {
            for (short i = 0; i < _xfs.Count; i++)
                if (_xfs[i].Equals(xf))
                    return i;

            return -1;
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                for (int i = 0; i < _xfs.Count; i++)
                {
                    bytes.Append(_xfs[i].Bytes);
                }

                return bytes;
            }
        }

        /// <summary>
        /// Gets the number of XF objects in this XFs collection.
        /// </summary>
        public object Count
        {
            get { return _xfs.Count; }
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// The root class of MyXls, containing general configuration properties and references to
    /// the Workbook as well as the MyOle2 Document which will contain and format the workbook
    /// when Sending or Saving.
    /// </summary>
    public class XlsDocument
    {
        private readonly MyOle2.Ole2Document _ole2Doc;
        private readonly Workbook _workbook;
        private readonly SummaryInformationSection _summaryInformation;
        private readonly DocumentSummaryInformationSection _documentSummaryInformation;

        private string _fileName = "Book1.xls";
        private bool _isLittleEndian = true;
        private bool _forceStandardOle2Stream = false;

        /// <summary>
        /// Initializes a new instance of the XlsDocument class.
        /// </summary>
        public XlsDocument()
        {
            _forceStandardOle2Stream = false;
            _isLittleEndian = true;

            _ole2Doc = new MyOle2.Ole2Document();
            SetOleDefaults();

            _summaryInformation = new SummaryInformationSection();
            _documentSummaryInformation = new DocumentSummaryInformationSection();

            _workbook = new Workbook(this);
        }

        /// <summary>
        /// Initializes a new XlsDocument from the provided file, reading in as much information
        /// from the file as possible and representing it appropriately with MyXls objects
        /// (Workbook, Worksheets, Cells, etc.).
        /// </summary>
        /// <param name="fileName">The name of the file to read into this XlsDocument.</param>
        public XlsDocument(string fileName)
            : this(fileName, null)
        { }

        internal XlsDocument(string fileName, Workbook.BytesReadCallback workbookBytesReadCallback)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Can't be null or Empty", "fileName");

            if (!File.Exists(fileName))
                throw new FileNotFoundException("Excel File not found", fileName);

            _ole2Doc = new Ole2Document();
            using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _ole2Doc.Load(fileStream);
            }

            //TODO: SummaryInformationSection and DocumentSummaryInformationSections should be read by MyOle2
            _workbook = new Workbook(this, _ole2Doc.Streams[_ole2Doc.Streams.GetIndex(Core.MyOle2.Directory.Biff8Workbook)].Bytes, workbookBytesReadCallback);
        }

        /// <summary>
        /// Initializes a new XlsDocument from the provided stream, reading in as much information
        /// from the file as possible and representing it appropriately with MyXls objects
        /// (Workbook, Worksheets, Cells, etc.).
        /// </summary>
        /// <param name="stream">The stream to read into this XlsDocument.</param>
        public XlsDocument(System.IO.Stream stream)
        {

            if (stream == null)
                throw new ArgumentException("Can't be null", "steam");

            _ole2Doc = new Ole2Document();
            _ole2Doc.Load(stream);

            //TODO: SummaryInformationSection and DocumentSummaryInformationSections should be read by MyOle2
            _workbook = new Workbook(this, _ole2Doc.Streams[_ole2Doc.Streams.GetIndex(Core.MyOle2.Directory.Biff8Workbook)].Bytes, null);
        }

        /// <summary>
        /// Initializes a new XlsDocument from the provided DataSet.  Each Table in the
        /// DataSet will be written to a separate Worksheet.  The data for each table will
        /// be written starting at A1 on each sheet, with a Header Row (A1 will be the
        /// name of the first Column, A2 will be the first value).
        /// </summary>
        /// <param name="dataSet">The DataSet to write to this XlsDocument.</param>
        public XlsDocument(DataSet dataSet)
            : this()
        {
            this.FileName = dataSet.DataSetName;
            foreach (DataTable table in dataSet.Tables)
            {
                Worksheet sheet = Workbook.Worksheets.Add(table.TableName);
                sheet.Write(table, 1, 1);
            }
        }

        /// <summary>
        /// Gets or sets the FileName (no path) for this XlsDocument (effective when sending
        /// to a Web client via the Send method).
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("FileName cannot be null or Empty");

                if (string.Compare(value.Substring(value.Length - 4), ".xls", true) != 0)
                    value = string.Format("{0}.xls", value);

                _fileName = value;
            }
        }

        private void SetOleDefaults()
        {
            //Can be any 16-byte value per OOo compdocfileformat.pdf
            _ole2Doc.DocUID = new byte[16];

            //Most common (BIFF8) value per OOo compdocfileformat.pdf
            _ole2Doc.SectorSize = 9;

            //Most common (BIFF8) value per OOo compdocfileformat.pdf
            _ole2Doc.ShortSectorSize = 6;

            //Most common (BIFF8) value per OOo compdocfileformat
            _ole2Doc.StandardStreamMinBytes = 4096;
        }

        /// <summary>
        /// Gets this Core.MyXls XlsDocument's OLE2 Document.
        /// </summary>
        public MyOle2.Ole2Document OLEDoc { get { return _ole2Doc; } }

        /// <summary>
        /// Gets this Core.MyXls XlsDocument's Workbook.
        /// </summary>
        public Workbook Workbook { get { return _workbook; } }

        /// <summary>
        /// Gets a Bytes object containing all the bytes of this OleDocument's stream.
        /// </summary>
        public Bytes Bytes
        {
            get
            {
                _ole2Doc.Streams.AddNamed(_workbook.Bytes, BIFF8.NameWorkbook, true);

                MetadataStream summaryInformationStream = new MetadataStream(_ole2Doc);
                summaryInformationStream.Sections.Add(_summaryInformation);
                _ole2Doc.Streams.AddNamed(GetStandardOLE2Stream(summaryInformationStream.Bytes), BIFF8.NameSummaryInformation, true);

                MetadataStream documentSummaryInformationStream = new MetadataStream(_ole2Doc);
                documentSummaryInformationStream.Sections.Add(_documentSummaryInformation);
                _ole2Doc.Streams.AddNamed(GetStandardOLE2Stream(documentSummaryInformationStream.Bytes), BIFF8.NameDocumentSummaryInformation, true);

                return _ole2Doc.Bytes;
            }
        }

        /// <summary>
        /// Gets the SummaryInformationSection for this XlsDocument.
        /// </summary>
        public SummaryInformationSection SummaryInformation
        {
            get { return _summaryInformation; }
        }

        /// <summary>
        /// Gets the DocumentSummaryInformationSection for this XlsDocument.
        /// </summary>
        public DocumentSummaryInformationSection DocumentSummaryInformation
        {
            get { return _documentSummaryInformation; }
        }

        /// <summary>
        /// Methods available to send XlsDocument to HTTP Client (Content-disposition header setting)
        /// </summary>
        public enum SendMethods
        {
            /// <summary>The client browser should try to open the file within browser window.</summary>
            Inline,

            /// <summary>The client browser should prompt to Save or Open the file.</summary>
            Attachment
        }

        private static string GetContentDisposition(SendMethods sendMethod)
        {
            if (sendMethod == SendMethods.Attachment)
                return "attachment";
            else if (sendMethod == SendMethods.Inline)
                return "inline";
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// Valid in an ASP.NET context only.  Sends this XlsDocument to the client with the 
        /// given FileName as an attachment file, via the HttpResponse object.  Clears
        /// the Response before sending and ends the Response after sending.
        /// </summary>
        public void Send()
        {
            Send(SendMethods.Attachment);
        }

        /// <summary>
        /// Valid in an ASP.NET context only.  Sends this XlsDocument to the client with the 
        /// given FileName as an attached or inline file, via the HttpResponse object.  Clears
        /// the Response before sending and ends the Response after sending.
        /// </summary>
        /// <param name="sendMethod">Method to use to send document.</param>
        public void Send(SendMethods sendMethod)
        {
            ////only applies in Classic ASP context?
            //System.Web.HttpContext context = System.Web.HttpContext.Current;
            //if (context == null)
            //    throw new ApplicationException("Current System.Web.HttpContext not found - Send failed.");

            //if (!context.Response.Buffer)
            //{
            //    context.Response.Buffer = true;
            //    context.Response.Clear();
            //}

            //context.Response.ContentType = "application/vnd.ms-excel";
            //context.Response.AddHeader("Content-Disposition", string.Format("{0};filename={1}", GetContentDisposition(sendMethod), FileName));
            //context.Response.Flush();
            //context.Response.BinaryWrite(Bytes.ByteArray);
        }

        /// <summary>
        /// Save this XlsDocument to the Current Directory. The FileName property will be used for
        /// the FileName.
        /// </summary>
        /// <param name="overwrite">Whether to overwrite if the specified file already exists.</param>
        public void Save(bool overwrite)
        {
            Save(null, overwrite);
        }

        /// <summary>
        /// Save this XlsDocument to the Current Directory.  The FileName property will be used
        /// for the FileName.  Will not overwrite.
        /// </summary>
        public void Save()
        {
            Save(null, false);
        }

        /// <summary>
        /// Save this XlsDocument to the given path.  The FileName property will be used for the
        /// FileName.  Will not overwrite.
        /// </summary>
        /// <param name="path">The Path to which to save this XlsDocument.</param>
        public void Save(string path)
        {
            Save(path, false);
        }

        /// <summary>
        /// Save this XlsDocument to the given path.  The FileName property will be used for the
        /// FileName.
        /// </summary>
        /// <param name="path">The Path to which to save this XlsDocument.</param>
        /// <param name="overwrite">Whether to overwrite if the specified file already exists.</param>
        public void Save(string path, bool overwrite)
        {
            path = path ?? Environment.CurrentDirectory;
            string fileName = Path.Combine(path, FileName);
            using (FileStream fs = new FileStream(fileName, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                Bytes.WriteToStream(fs);
                fs.Flush();
            }
        }

        /// <summary>
        /// Saves the XlsDocument to the given stream
        /// </summary>
        /// <param name="outStream"></param>
        public void Save(System.IO.Stream outStream)
        {
            Bytes.WriteToStream(outStream);
        }

        /// <summary>
        /// Gets whether this XlsDocument is Little Endian.  In the current implementation of
        /// MyXLS, this is always true.
        /// </summary>
        public bool IsLittleEndian
        {
            get { return _isLittleEndian; }
        }

        /// <summary>
        /// Gets or sets whether to force the XlsDocument data to be padded to the length
        /// of a Standard Stream in its MyOle2 document container, if it is less than 
        /// standard length without padding.
        /// </summary>
        public bool ForceStandardOle2Stream
        {
            get { return _forceStandardOle2Stream; }
            set { _forceStandardOle2Stream = value; }
        }

        internal Bytes GetStandardOLE2Stream(Bytes bytes)
        {
            uint standardLength = _ole2Doc.StandardStreamMinBytes;
            uint padLength = standardLength = ((uint)bytes.Length % standardLength);
            if (padLength < standardLength)
                bytes.Append(new byte[padLength]);
            return bytes;
        }

        internal static Bytes GetUnicodeString(string text, int lengthBits)
        {
            int textLength;
            int limit = lengthBits == 8 ? byte.MaxValue : ushort.MaxValue;
            byte[] binaryLength = new byte[0];
            byte[] compression;
            byte[] compressedText = new byte[0];

            textLength = text.Length;
            if (textLength > limit)
                text = text.Substring(0, limit); //NOTE: Should throw Exception here?

            if (limit == 255)
                binaryLength = new byte[1] { (byte)text.Length };
            else if (limit == 65535)
                binaryLength = BitConverter.GetBytes((ushort)text.Length);

            if (IsCompressible(text))
            {
                compression = new byte[1];
                char[] chars = text.ToCharArray();
                compressedText = new byte[chars.Length];
                for (int i = 0; i < chars.Length; i++)
                    compressedText[i] = (byte)chars[i];
            }
            else
            {
                compression = new byte[1] { 1 };
            }

            Bytes bytes = new Bytes();
            bytes.Append(binaryLength);
            bytes.Append(compression);
            if (compressedText.Length > 0)
                bytes.Append(compressedText);
            else
                bytes.Append(Encoding.Unicode.GetBytes(text));
            return bytes;
        }

        //TODO: Create optional setting for this optimization (to force all strings to unicode
        //storage so this check doesn't reduce performance - similar to Workbook.ShareStrings)
        private static bool IsCompressible(string text)
        {
            byte[] textBytes = Encoding.Unicode.GetBytes(text);

            for (int i = 1; i < textBytes.Length; i += 2)
            {
                if (textBytes[i] != 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a new XF (Formatting) object for use on this document.
        /// </summary>
        /// <returns>New XF formatting object.</returns>
        public XF NewXF()
        {
            return new XF(this);
        }
    }
}
namespace Core.MyXls
{
    /// <summary>
    /// This is not done by any stretch of the imagination.  What a can of worms!
    /// </summary>
    internal class XlsText
    {
        private string _text = null;

        public XlsText(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        internal class FormattingRun
        {
            private ushort _xfId = 0;
            private ushort _startOffset = 0;

            public FormattingRun(XF xf, ushort startOffset)
            {
                _xfId = xf.Id;
                _startOffset = startOffset;
            }

            public XF ExtendedFormat
            {
                //NOTE: Probably should be able to have a getter here - if we require a Workbook or XlsDocument ref in the constructor
                set { _xfId = value.Id; }
            }

            public ushort StartOffset
            {
                get { return _startOffset; }
                set { _startOffset = value; }
            }
        }
    }
}
namespace Core.MyXls.ByteUtil
{
    public partial class Bytes
    {
        /// <summary>
        /// Gets a Bits object representing the Bits comprising these Bytes.
        /// </summary>
        /// <returns>A Bits object representing the Bits comprising these Bytes.</returns>
        public Bits GetBits()
        {
            return new Bits(this);
        }

        /// <summary>
        /// A helper class to manage bit (bool) arrays and lists, encapsulating helper
        /// methods for subdividing and converting to/from bytes.
        /// </summary>
        public class Bits
        {
            private bool[] _bits = new bool[0];

            /// <summary>
            /// Initializes a new instance of the Bits class from the given Bytes.
            /// </summary>
            /// <param name="bytes">The Bytes whose bits this Bits object will represent.</param>
            public Bits(Bytes bytes)
            {
                byte[] byteArray = bytes.ByteArray;
                _bits = new bool[byteArray.Length * 8];
                for (byte i = 0; i < byteArray.Length; i++)
                {
                    SetBits(i, byteArray[i]);
                }
            }

            /// <summary>
            /// Initializes a new instance of the Bits class from the given byte array.
            /// </summary>
            /// <param name="bits">The bytes whose bits this Bits object will represent.</param>
            public Bits(bool[] bits)
            {
                _bits = bits;
            }

            private void SetBits(byte byteIndex, byte fromByte)
            {
                for (byte b = 7; b >= 0 && b < 255; b--)
                {
                    byte value = (byte)Math.Pow(2, b);
                    if (fromByte >= value)
                    {
                        _bits[(byteIndex * 8) + b] = true;
                        fromByte -= value;
                    }
                }
            }

            /// <summary>
            /// Prepends the specified bit to the beginning of this Bits collection.
            /// </summary>
            /// <param name="bit">The bit to prepend.</param>
            public void Prepend(bool bit)
            {
                bool[] newBits = new bool[_bits.Length + 1];
                _bits.CopyTo(newBits, 1);
                _bits = newBits;
                _bits[0] = bit;
            }

            //public void Append

            /// <summary>
            /// Gets a new Bits object containing the first getLength bits in this Bits object.
            /// </summary>
            /// <param name="getLength">The number of bits to return from the beginning of
            /// this Bits object in a new Bits object.</param>
            /// <returns>A new Bits object containing the provided number of bits from the beginning
            /// of this Bits object.</returns>
            public Bits Get(int getLength)
            {
                return Get(0, getLength);
            }

            /// <summary>
            /// Gets a new Bits object containing intLength bits in this Bits object,
            /// beginning at offset.
            /// </summary>
            /// <param name="offset">The start index from which the new Bits object is to be returned
            /// from this Bits object.</param>
            /// <param name="getLength">The number of bits to return from this Bits object 
            /// in a new Bits object.</param>
            /// <returns>A new Bits object containing the provided number of bits from this Bits 
            /// object, beginning at the provided offset.</returns>
            public Bits Get(int offset, int getLength)
            {
                if (offset < 0)
                    throw new ArgumentOutOfRangeException(string.Format("offset {0} must be >= 0", offset));

                if (getLength < 0)
                    throw new ArgumentOutOfRangeException(string.Format("getLength {0} must be >= 0", getLength));

                if (offset >= Length)
                    throw new ArgumentOutOfRangeException(string.Format("offset {0} must be < Length {1}", offset, Length));

                if ((getLength + offset) > Length)
                    throw new ArgumentOutOfRangeException(
                        string.Format("offset {0} + getLength {1} = {2} must be < Length {3}", offset, getLength, offset + getLength,
                                      Length));

                bool[] subBits = new bool[getLength];
                Array.Copy(_bits, offset, subBits, 0, getLength);
                return new Bits(subBits);
            }

            /// <summary>
            /// Gets the length or number of bits in this Bits object.
            /// </summary>
            public int Length
            {
                get { return _bits.Length; }
            }

            /// <summary>
            /// Gets a bool[] representing the bits in this Bits object.
            /// </summary>
            public bool[] Values
            {
                get { return _bits; }
            }

            /// <summary>
            /// Calculates and returns the uint which these bits represnt.
            /// </summary>
            /// <returns>The uint value represented by these bits.</returns>
            public uint ToUInt32()
            {
                int length = _bits.Length;
                if (length > 32)
                    throw new ApplicationException(string.Format("Length {0} must be <= 32", length));
                uint value = 0;
                for (int i = (length - 1); i >= 0; i--)
                {
                    if (_bits[i])
                        value += (uint)Math.Pow(2, i);
                }
                return value;
            }

            /// <summary>
            /// Calculates and returns the int which these bits represent.
            /// </summary>
            /// <returns>The int value represented by these bits.</returns>
            public int ToInt32()
            {
                int length = _bits.Length;
                if (length > 32)
                    throw new ApplicationException(string.Format("Length {0} must be <= 32", length));
                int value = 0;
                for (int i = (length - 1); i >= 0; i--)
                {
                    if (_bits[i])
                        value += (int)Math.Pow(2, i);
                }
                return value;
            }

            /// <summary>
            /// Calculates and returns the ushort which these bits represent.
            /// </summary>
            /// <returns>The ushort value represented by these bits.</returns>
            public ushort ToUInt16()
            {
                int length = _bits.Length;
                if (length > 16)
                    throw new ApplicationException(string.Format("Length {0} must be <= 16", length));
                ushort value = 0;
                for (int i = 0; i < length; i++)
                {
                    if (_bits[i])
                        value += (ushort)Math.Pow(2, i);
                }
                return value;
            }

            /// <summary>
            /// Calculates and returns the ulong which these bits represent.
            /// </summary>
            /// <returns>The ulong value represented by these bits.</returns>
            public ulong ToUInt64()
            {
                int length = _bits.Length;
                if (length > 64)
                    throw new ApplicationException(string.Format("Length {0} must be <= 64", length));
                ushort value = 0;
                for (int i = 0; i < length; i++)
                {
                    if (_bits[i])
                        value += (ushort)Math.Pow(2, i);
                }
                return value;
            }

            /// <summary>
            /// Calculates and returns a Bytes object containing the bytes which these bits represent.
            /// </summary>
            /// <returns>A Bytes object containing the bytes represented by these bits.</returns>
            public Bytes GetBytes()
            {
                byte[] bytes = new byte[(int)Math.Ceiling(_bits.Length / 8.0)];
                for (int i = (bytes.Length - 1); i >= 0; i--)
                {
                    byte b = 0x00;
                    for (int j = 7; j >= 0; j--)
                    {
                        int index = (8 * i + j);
                        if (index < _bits.Length && _bits[index])
                            b += (byte)Math.Pow(2, j);
                    }
                    bytes[i] = b;
                }
                return new Bytes(bytes);
            }

            /// <summary>
            /// Calculates and returns the double precision floating-point value which these bits represent.
            /// </summary>
            /// <returns>The double precision floating-point value represented by these bits.</returns>
            public double ToDouble()
            {
                List<bool> bitList = new List<bool>();
                bitList.AddRange(new bool[64 - _bits.Length]);
                bitList.AddRange(_bits);
                byte[] bytes = new byte[8];
                for (int i = 7; i >= 0; i--)
                {
                    for (int j = 7; j >= 0; j--)
                    {
                        if (bitList[8 * i + j])
                            bytes[i] += (byte)Math.Pow(2, j);
                    }
                }
                return BitConverter.ToDouble(bytes, 0);
            }
        }
    }
}
namespace Core.MyXls.ByteUtil
{
    using System.IO;
    /// <summary>
    /// A helper class to manage byte arrays, allowing subdividing and combining
    /// arrays without incurring the cost of copying the bytes from one array to
    /// another.
    /// </summary>
    public partial class Bytes
    {
        private byte[] _byteArray = null;
        internal List<Bytes> _bytesList = null;
        private int _length = 0;

        /// <summary>
        /// Initializes a new, empty, instance of the Bytes class.
        /// </summary>
        public Bytes() { }

        /// <summary>
        /// Initializes a new instance of the Bytes class containing the provided byte.
        /// </summary>
        /// <param name="b">byte with which to initialize this Bytes instance.</param>
        public Bytes(byte b)
            : this(new byte[] { b })
        {
        }

        /// <summary>
        /// Initializes a new instance of the Bytes class containing the provided byte
        /// array.
        /// </summary>
        /// <param name="byteArray">Array of bytes to initialize this Bytes instance.</param>
        public Bytes(byte[] byteArray)
            : this()
        {
            CheckNewLength(byteArray);
            _byteArray = byteArray;
            _length = byteArray.Length;
        }

        /// <summary>
        /// Initializes a new instance of the Bytes class containing the same bytes as the
        /// provided Bytes object.
        /// </summary>
        /// <param name="bytes">Bytes class to initialize this Bytes instance.</param>
        public Bytes(Bytes bytes)
            : this()
        {
            CheckNewLength(bytes);
            _bytesList = new List<Bytes>();
            _bytesList.Add(bytes);
            _length = bytes.Length;
        }

        /// <summary>
        /// Gets the length or number of bytes in this Bytes object.
        /// </summary>
        public int Length { get { return _length; } }

        internal bool IsEmpty { get { return _length == 0; } }
        internal bool IsArray { get { return _byteArray != null; } }

        /// <summary>
        /// Appends the provided byte array at the end of the existing bytes in this
        /// Bytes object.
        /// </summary>
        /// <param name="byteArray">The byte array to append at the end of this Bytes
        /// object.</param>
        public void Append(byte[] byteArray)
        {
            if (byteArray.Length == 0)
                return;

            CheckNewLength(byteArray);

            if (IsEmpty)
            {
                _byteArray = byteArray;
            }
            else if (IsArray)
            {
                ConvertToList();
                _bytesList.Add(new Bytes(byteArray));
            }
            else
            {
                _bytesList.Add(new Bytes(byteArray));
            }

            _length += byteArray.Length;
        }

        /// <summary>
        /// Appends a single byte to the end of this Bytes object.
        /// </summary>
        /// <param name="b">They byte to append to the end of this object.</param>
        public void Append(byte b)
        {
            Append(new byte[] { b });
        }

        /// <summary>
        /// Appends the contents of the provided Bytes object to the end of this
        /// Bytes object.
        /// </summary>
        /// <param name="bytes">The Bytes object whose contents are to be appended
        /// at the end of this Bytes object.</param>
        public void Append(Bytes bytes)
        {
            if (bytes.Length == 0)
                return;

            CheckNewLength(bytes);

            if (IsEmpty)
            {
                _bytesList = new List<Bytes>();
                _bytesList.Add(bytes);
            }
            else if (IsArray)
            {
                ConvertToList();
                _bytesList.Add(bytes);
            }
            else
            {
                _bytesList.Add(bytes);
            }

            _length += bytes.Length;
        }

        /// <summary>
        /// Prepends the provided byte array at the beginning of the existing bytes in this
        /// Bytes object.
        /// </summary>
        /// <param name="byteArray">The byte array to prepend at the beginning of this Bytes
        /// object.</param>
        public void Prepend(byte[] byteArray)
        {
            Prepend(new Bytes(byteArray));
        }

        /// <summary>
        /// Prepends the contents of the provided Bytes object to the beginning of this
        /// Bytes object.
        /// </summary>
        /// <param name="bytes">The Bytes object whose contents are to be prepended
        /// at the beginning of this Bytes object.</param>
        public void Prepend(Bytes bytes)
        {
            if (bytes.Length == 0)
                return;

            CheckNewLength(bytes);

            if (IsEmpty)
            {
                Append(bytes);
                return;
            }

            if (IsArray)
                ConvertToList();

            _bytesList.Insert(0, bytes);

            _length += bytes.Length;
        }

        /// <summary>
        /// Gets a byte array containing all bytes in this Bytes object.
        /// </summary>
        public byte[] ByteArray
        {
            get
            {
                if (IsEmpty)
                    return new byte[0];

                MemoryStream memoryStream = new MemoryStream();
                WriteToStream(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Gets a Bytes object containing the first intLength bytes in this Bytes object.
        /// </summary>
        /// <param name="getLength">The number of bytes to return from the beginning of
        /// this Bytes object in a new Bytes object.</param>
        /// <returns>A Bytes object containing the provided number of bytes from the beginning
        /// of this Bytes object.</returns>
        public Bytes Get(int getLength)
        {
            return Get(0, getLength);
        }

        /// <summary>
        /// Gets a Bytes object containing intLength bytes in this Bytes object,
        /// beginning at offset.
        /// </summary>
        /// <param name="offset">The index at which the sub-array of bytes is to be returned
        /// from this Bytes object in a new Bytes object.</param>
        /// <param name="getLength">The number of bytes to return from this Bytes object 
        /// in a new Bytes object.</param>
        /// <returns>A Bytes object containing the provided number of bytes from this Bytes 
        /// object, beginning at the provided offset.</returns>
        public Bytes Get(int offset, int getLength)
        {
            Bytes bytes = new Bytes();

            if (getLength == 0)
                return bytes;

            Get(offset, getLength, bytes);

            return bytes;
        }

        private void Get(int offset, int getLength, Bytes intoBytes)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(string.Format("offset {0} must be >= 0", offset));

            if (getLength < 0)
                throw new ArgumentOutOfRangeException(string.Format("getLength {0} must be >= 0", getLength));

            if (offset >= Length)
                throw new ArgumentOutOfRangeException(string.Format("offset {0} must be < Length {1}", offset, Length));

            if ((getLength + offset) > Length)
                throw new ArgumentOutOfRangeException(
                    string.Format("offset {0} + getLength {1} = {2} must be < Length {3}", offset, getLength, offset + getLength,
                                  Length));

            if (IsArray)
            {
                if (offset == 0 && getLength == Length)
                {
                    intoBytes.Append(_byteArray);
                    return;
                }

                intoBytes.Append(MidByteArray(_byteArray, offset, getLength));
                return;
            }

            foreach (Bytes bytes in _bytesList)
            {
                if (bytes.Length <= offset)
                {
                    offset -= bytes.Length;
                    continue;
                }

                if (bytes.Length >= (offset + getLength))
                {
                    bytes.Get(offset, getLength, intoBytes);
                    return;
                }

                int lengthToGet = bytes.Length - offset;
                bytes.Get(offset, lengthToGet, intoBytes);
                getLength -= lengthToGet;
                offset = 0;
            }
        }

        internal static byte[] MidByteArray(byte[] byteArray, int offset, int length)
        {
            if (offset >= byteArray.Length)
                throw new ArgumentOutOfRangeException(string.Format("offset {0} must be less than byteArray.Length {1}", offset, byteArray.Length));

            if (offset + length > byteArray.Length)
                throw new ArgumentOutOfRangeException(string.Format("offset {0} + length {1} must be <= byteArray.Length {2}", offset, length, byteArray.Length));

            if (offset == 0 && length == byteArray.Length)
                return byteArray;

            byte[] subArray = new byte[length];
            for (int i = 0; i < length; i++)
                subArray[i] = byteArray[offset + i];
            return subArray;
        }

        internal void WriteToStream(Stream stream)
        {
            if (IsEmpty)
                return;
            else if (IsArray)
            {
                stream.Write(_byteArray, 0, _length);
                return;
            }
            else
            {
                foreach (Bytes bytes in _bytesList)
                {
                    bytes.WriteToStream(stream);
                }
                return;
            }
        }

        private void CheckNewLength(byte[] withAddition)
        {
            CheckNewLength(withAddition.Length);
        }

        private void CheckNewLength(Bytes withAddition)
        {
            CheckNewLength(withAddition.Length);
        }

        private void CheckNewLength(int withAddition)
        {
            if ((_length + withAddition) > int.MaxValue)
                throw new Exception(
                    string.Format("Addition of {0} bytes would exceed current limit of {1} bytes", withAddition, int.MaxValue));
        }

        private void ConvertToList()
        {
            _bytesList = new List<Bytes>();

            if (IsEmpty)
                return;

            Bytes newBytes = new Bytes(_byteArray);
            _byteArray = null;
            _bytesList.Add(newBytes);
        }

        /// <summary>
        /// Determines whether the two provided byte arrays are equal by byte-values.
        /// </summary>
        /// <param name="a">The first byte array to compare.</param>
        /// <param name="b">The second byte array to copare.</param>
        /// <returns>true if a and b are byte-equal, false otherwise</returns>
        public static bool AreEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;

            return true;
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Represents the Directory stream of an OLE2 Document.
    /// </summary>
    public class Directory
    {
        /// <summary>
        /// The name of the Root entry of an OLE2 Directory.
        /// </summary>
        public static readonly byte[] RootName = new byte[] { 0x52, 0x00, 0x6f, 0x00, 0x6f, 0x00, 0x74, 0x00, 0x20, 0x00, 0x45, 0x00, 0x6e, 0x00, 0x74, 0x00, 0x72, 0x00, 0x79, 0x00, 0x00, 0x00 };

        /// <summary>
        /// The name of a BIFF8 Workbook Stream.
        /// </summary>
        public static readonly byte[] Biff8Workbook = new byte[] { 0x57, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6B, 0x00, 0x62, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6B, 0x00, 0x00, 0x00 };

        private static readonly byte[] StreamNameSumaryInformation = new byte[] {
            0x05, 0x00, 0x53, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00,
            0x49, 0x00, 0x6E, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x74, 0x00,
            0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00};

        private static readonly byte[] StreamNameDocumentSummaryInformation = new byte[] {
            0x05, 0x00, 0x44, 0x00, 0x6F, 0x00, 0x63, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x6E, 0x00,
            0x74, 0x00, 0x53, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00,
            0x49, 0x00, 0x6E, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x74, 0x00,
            0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00};

        private readonly Ole2Document _doc;

        /// <summary>
        /// Initializes a new instance of the Directory class for the provided Doc object.
        /// </summary>
        /// <param name="doc">The Doc object to which this new Directory is to belong.</param>
        public Directory(Ole2Document doc)
        {
            _doc = doc;
        }

        //TODO: I think this should be Streams.Count + Storages.Count + 1 (for Root Entry)
        //But this is okay until User Storage support is added (with Red-Black Tree Implementation)
        /// <summary>
        /// Gets the number of entries in this Directory object (not including blank/filler entries).
        /// </summary>
        public int EntryCount
        {
            get { return _doc.Streams.Count + 1; }
        }

        /// <summary>
        /// Gets the number of Sectors required by this Directory.
        /// </summary>
        public int SectorCount
        {
            get
            {
                return (int)Math.Ceiling((decimal)EntryCount / EntriesPerSector);
            }
        }

        private int EntriesPerSector
        {
            get { return _doc.BytesPerSector / 128; }
        }

        /// <summary>
        /// Gets the first SID of this Directory's stream.
        /// </summary>
        public int SID0
        {
            get
            {
                int sid0;
                if (_doc.SSAT.SID0 != -2)
                    sid0 = _doc.SSAT.SID0 + _doc.SSAT.SectorCount;
                else
                    sid0 = _doc.SAT.SID0 + _doc.SAT.SectorCount;
                sid0 += _doc.Streams.SectorCount;
                return sid0;
            }
        }

        internal Bytes Bytes
        {
            get
            {
                int streamCount = _doc.Streams.Count;

                Bytes bytes = new Bytes();

                bytes.Append(StreamDirectoryBytes(_doc.Streams.ShortSectorStorage));
                for (int i = 1; i <= streamCount; i++)
                    bytes.Append(StreamDirectoryBytes(_doc.Streams[i]));

                int padSectors = (streamCount + 1) % EntriesPerSector;
                if (padSectors > 0)
                    padSectors = EntriesPerSector - padSectors;
                for (int i = 1; i <= padSectors; i++)
                    bytes.Append(BlankEntryByteArray);

                return bytes;
            }
        }

        private static readonly byte[] BlankEntryByteArray = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

        private Bytes StreamDirectoryBytes(Stream stream)
        {
            Bytes bytes = new Bytes();

            //Stream Name
            bytes.Append(stream.Name);

            //Stream Name buffer fill
            bytes.Append(new byte[64 - bytes.Length]);

            //Stream Name length (including ending 0x00)
            bytes.Append(BitConverter.GetBytes((ushort)stream.Name.Length));

            //Type of entry {&H00 -> Empty, &H01 -> User Storage,
            //		&H02 -> User Stream, &H03 -> LockBytes (unknown),
            //		&H04 -> Property (unknown), &H05 -> Root storage}
            //TODO: UnHack this
            bytes.Append(HackDirectoryType(stream.Name));

            //TODO: Implement Red-Black Tree Node color {&H00 -> Red, &H01 -> Black} (Doesn't matter)
            bytes.Append(0x01);

            //TODO: UnHack Red-Black Tree Left-Child Node DID (-1 if no left child)
            bytes.Append(BitConverter.GetBytes(HackDirectoryDID(stream.Name, "LeftDID")));

            //TODO: UnHack Red-Black Tree Right-Child Node DID (-1 if no right child)
            bytes.Append(BitConverter.GetBytes(HackDirectoryDID(stream.Name, "RightDID")));

            //TODO: UnHack Storage Member Red-Black Tree Root Node DID (-1 if not storage)
            bytes.Append(BitConverter.GetBytes(HackDirectoryDID(stream.Name, "RootDID")));

            //Unique identifier for storage (Doesn't matter)
            bytes.Append(new byte[16]);

            //User flags (Doesn't matter)
            bytes.Append(new byte[4]);

            //Entry Creation Timestamp (Can be all 0's)
            bytes.Append(new byte[8]);

            //Entry Modification Timestamp (Can be all 0's)
            bytes.Append(new byte[8]);

            //SID of Stream's First Sector (for Short or Standard Streams)
            bytes.Append(BitConverter.GetBytes(stream.SID0));

            //Stream Size in Bytes (0 if storage, but not Root Storage entry)
            bytes.Append(BitConverter.GetBytes(stream.ByteCount));

            //Not used
            bytes.Append(new byte[4]);

            return bytes;
        }

        private static Bytes HackDirectoryType(byte[] streamName)
        {
            if (Bytes.AreEqual(streamName, RootName))
            {
                return new Bytes(new byte[] { 0x05 });
            }
            else if (Bytes.AreEqual(streamName, Biff8Workbook))
            {
                return new Bytes(new byte[] { 0x02 });
            }
            else if (Bytes.AreEqual(streamName, StreamNameSumaryInformation))
            {
                return new Bytes(new byte[] { 0x02 });
            }
            else if (Bytes.AreEqual(streamName, StreamNameDocumentSummaryInformation))
            {
                return new Bytes(new byte[] { 0x02 });
            }
            else
            {
                return new Bytes(new byte[] { 0xFF });
            }
        }

        private static int HackDirectoryDID(byte[] streamName, string didType)
        {
            if (Bytes.AreEqual(streamName, RootName))
            {
                switch (didType)
                {
                    case "LeftDID":
                        return -1;
                    case "RightDID":
                        return -1;
                    case "RootDID":
                        return 2;
                }
            }
            else if (Bytes.AreEqual(streamName, Biff8Workbook))
            {
                switch (didType)
                {
                    case "LeftDID":
                        return -1;
                    case "RightDID":
                        return -1;
                    case "RootDID":
                        return -1;
                }
            }
            else if (Bytes.AreEqual(streamName, StreamNameSumaryInformation))
            {
                switch (didType)
                {
                    case "LeftDID":
                        return 1;
                    case "RightDID":
                        return 3;
                    case "RootDID":
                        return -1;
                }
            }
            else if (Bytes.AreEqual(streamName, new byte[] { //BIFF8 '&05HDocumentSummaryInformation'
			    0x05, 0x00, 0x44, 0x00, 0x6F, 0x00, 0x63, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x6E, 0x00,
                0x74, 0x00, 0x53, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00,
                0x49, 0x00, 0x6E, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x74, 0x00,
                0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00}))
            {
                switch (didType)
                {
                    case "LeftDID":
                        return -1;
                    case "RightDID":
                        return -1;
                    case "RootDID":
                        return -1;
                }
            }
            else
            {
                switch (didType)
                {
                    case "LeftDID":
                        return 1000000; //Huh?  I think these are dummy values
                    case "RightDID":
                        return 1000000;
                    case "RootDID":
                        return 1000000;
                }
            }

            throw new Exception(string.Format("Unexpected didType {0} for HackDirectoryDID", didType));
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Contains properties and information about the Header of an OLE2 document.
    /// </summary>
    public class Header
    {
        private static readonly byte[] LITTLE_ENDIAN = new byte[2] { 0xFE, 0xFF };
        private static readonly byte[] BIG_ENDIAN = new byte[2] { 0xFF, 0xFE };

        private readonly Ole2Document _doc;

        /// <summary>
        /// Initializes a new instance of the Header class for the given Document object.
        /// </summary>
        /// <param name="doc">The parent OleDocument object for this Header object.</param>
        public Header(Ole2Document doc)
        {
            _doc = doc;

            SetDefaults();
        }

        private void SetDefaults()
        {
            //empty in original
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                bytes.Append(_doc.DocFileID);
                bytes.Append(_doc.DocUID);
                bytes.Append(_doc.FileFormatRevision);
                bytes.Append(_doc.FileFormatVersion);
                bytes.Append(_doc.IsLittleEndian ? LITTLE_ENDIAN : BIG_ENDIAN);
                bytes.Append(BitConverter.GetBytes(_doc.SectorSize));
                bytes.Append(BitConverter.GetBytes(_doc.ShortSectorSize));
                bytes.Append(_doc.Blank1);
                bytes.Append(BitConverter.GetBytes(_doc.SAT.SectorCount));
                bytes.Append(BitConverter.GetBytes(_doc.Directory.SID0));
                bytes.Append(_doc.Blank2);
                bytes.Append(BitConverter.GetBytes(_doc.StandardStreamMinBytes));
                bytes.Append(BitConverter.GetBytes(_doc.SSAT.SID0));
                bytes.Append(BitConverter.GetBytes(_doc.SSAT.SectorCount));
                bytes.Append(BitConverter.GetBytes(_doc.MSAT.SID0));
                bytes.Append(BitConverter.GetBytes(_doc.MSAT.SectorCount));
                bytes.Append(_doc.MSAT.Head);

                return bytes;
            }
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Represents the MSAT (Master Sector Allocation Table) for an OLE2 Document.
    /// </summary>
    public class Msat
    {
        private readonly Ole2Document _doc;

        /// <summary>
        /// Initializes a new instance of the Msat class for the given Doc object.
        /// </summary>
        /// <param name="doc">The parent Doc object for the new Msat object.</param>
        public Msat(Ole2Document doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Gets the count of MSAT sectors used by this MSAT.  This value is zero if
        /// the MSAT contains &lt;= 109 SIDs, which will be contained in the Header sector.
        /// </summary>
        public int SectorCount
        {
            get
            {
                int sectorCount = _doc.SAT.SectorCount;

                if (sectorCount <= 109)
                    return 0;

                sectorCount -= 109;
                if ((decimal)sectorCount % 127 == 0)
                    return (int)Math.Floor((decimal)sectorCount / 127);
                else
                    return ((int)Math.Floor((decimal)sectorCount / 127)) + 1;
            }
        }

        /// <summary>
        /// Gets the SID0, or SID of the first sector, of the MSAT.  If SectorCount
        /// is 0 (zero), the SID0 will return -2.  Currently, 0 is the only value 
        /// other than -2 returned, as the current MyOle2 implementation always
        /// places any MSAT sectors as the first sectors in the document.
        /// </summary>
        public int SID0
        {
            get
            {
                if (SectorCount == 0)
                    return -2;
                else
                    return 0; //TODO: See comment in Ole2Document.Bytes Property Get
            }
        }

        internal Bytes Head
        {
            get
            {
                return SectorBinData(0);
            }
        }

        private Bytes SectorBinData(int sectorIndex)
        {
            if (0 > sectorIndex || sectorIndex > SectorCount)
                throw new ArgumentOutOfRangeException(string.Format("sectorIndex must be >= 0 and <= SectorCount {0}", SectorCount));

            int satSectors, satSid0, startSector, stopSector;

            Bytes bytes = new Bytes();

            satSectors = _doc.SAT.SectorCount;
            satSid0 = _doc.SAT.SID0;

            if (sectorIndex == 0)
            {
                startSector = 1;
                stopSector = 109;
            }
            else
            {
                startSector = 110 + ((sectorIndex - 1) * 127);
                stopSector = startSector + 126;
            }

            for (int i = startSector; i <= stopSector; i++)
            {
                if (i < (satSectors + 1))
                    bytes.Append(BitConverter.GetBytes((int)(satSid0 + (i - 1))));
                else
                    bytes.Append(BitConverter.GetBytes((int)-1));
            }

            if (sectorIndex > 0)
            {
                if (stopSector >= satSectors)
                    bytes.Append(BitConverter.GetBytes((int)-2));
                else
                    bytes.Append(BitConverter.GetBytes((int)(SID0 + sectorIndex)));
            }

            return bytes;
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();
                int sectorCount = SectorCount;

                for (int i = 1; i <= sectorCount; i++)
                    bytes.Append(SectorBinData(i));

                return bytes;
            }
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Represents an OLE2 Compound Document format Document.
    /// </summary>
    public class Ole2Document
    {
        private readonly Header _header;
        private readonly Msat _msat;
        private readonly Sat _sat;
        private readonly Ssat _ssat;
        private readonly Directory _directory;
        private readonly Streams _streams;

        private byte[] _docFileID;
        private byte[] _docUID;
        private byte[] _fileFormatRevision;
        private byte[] _fileFormatVersion;
        private bool _isLittleEndian;
        private ushort _sectorSize; //in power of 2 (min = 7)
        private ushort _shortSectorSize; //in power of 2 (max = ShortSectorSize)
        private byte[] _blank1;
        private byte[] _blank2;
        private uint _standardStreamMinBytes; //in bytes

        private int _bytesPerSector;
        private int _bytesPerShortSector;

        /// <summary>
        /// Initializes a new, blank, Ole2Document object.
        /// </summary>
        public Ole2Document()
        {
            SetDefaults();

            _header = new Header(this);
            _msat = new Msat(this);
            _sat = new Sat(this);
            _ssat = new Ssat(this);
            _directory = new Directory(this);
            _streams = new Streams(this);

            _streams.AddNamed(new Bytes(), Directory.RootName);
        }

        /// <summary>
        /// Gets this Ole2Document's Header object.
        /// </summary>
        public Header Header { get { return _header; } }

        /// <summary>
        /// Gets this Ole2Document's MSAT (Main Sector Allocation Table) object.
        /// </summary>
        public Msat MSAT { get { return _msat; } }

        /// <summary>
        /// Gets this Ole2Document's Directory object.
        /// </summary>
        public Directory Directory { get { return _directory; } }

        /// <summary>
        /// Gets this Ole2Document's Streams collection.
        /// </summary>
        public Streams Streams { get { return _streams; } }

        /// <summary>
        /// Gets this Ole2Document's SSAT (Short Sector Allocation Table) object.
        /// </summary>
        public Ssat SSAT { get { return _ssat; } }

        /// <summary>
        /// Gets this Ole2Document's SAT (Sector Allocation Table) object.
        /// </summary>
        public Sat SAT { get { return _sat; } }

        /// <summary>
        /// Gets the number of bytes per Short Sector in this Ole2Document (set with ShortSectorSize).
        /// </summary>
        public int BytesPerShortSector
        {
            get { return _bytesPerShortSector; }
        }

        /// <summary>
        /// Gets the number of bytes per Sector in this Ole2Document (set with SectorSize).
        /// </summary>
        public int BytesPerSector
        {
            get { return _bytesPerSector; }
        }

        /// <summary>
        /// Gets or sets the number of bytes per Standard (not short) Stream in this Ole2Document.
        /// This is not normally changed from its default.
        /// </summary>
        public uint StandardStreamMinBytes
        {
            get { return _standardStreamMinBytes; }
            set
            {
                if (value <= 2)
                    throw new ArgumentOutOfRangeException("value", "must be > 2");

                _standardStreamMinBytes = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ole2Document FileID of this Ole2Document.
        /// This is not normally changed from its default.
        /// </summary>
        public byte[] DocFileID
        {
            get { return _docFileID; }
            set
            {
                if (value.Length != 8)
                    throw new ArgumentOutOfRangeException("value", "must be 8 bytes in length");

                _docFileID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ole2Document UID of this Ole2Document.
        /// This is not normally changed from its default.
        /// </summary>
        public byte[] DocUID
        {
            get { return _docUID; }
            set
            {
                if (value.Length != 16)
                    throw new ArgumentOutOfRangeException("value", "must be 16 bytes in length");

                _docUID = value;
            }
        }

        /// <summary>
        /// Gets or sets the FileFormatRevision of this Ole2Document.
        /// This is not normally changed from its default.
        /// </summary>
        public byte[] FileFormatRevision
        {
            get { return _fileFormatRevision; }
            set
            {
                if (value.Length != 2)
                    throw new ArgumentOutOfRangeException("value", "must be 2 bytes in length");

                _fileFormatRevision = value;
            }
        }

        /// <summary>
        /// Gets or sets the FileFormatVersion of this Ole2Document.
        /// This is not normally changed from its default.
        /// </summary>
        public byte[] FileFormatVersion
        {
            get { return _fileFormatVersion; }
            set
            {
                if (value.Length != 2)
                    throw new ArgumentOutOfRangeException("value", "must be 2 bytes in length");

                _fileFormatVersion = value;
            }
        }


        /// <summary>
        /// Gets whether this Ole2Document should be encoded in Little Endian (or
        /// Big Endian) format.  Currently only Little Endian is supported.
        /// </summary>
        public bool IsLittleEndian
        {
            get { return _isLittleEndian; }
            set { if (value == false) throw new NotSupportedException("Big Endian not currently supported"); _isLittleEndian = value; }
        }

        /// <summary>
        /// Gets or sets the Sector Size of this Ole2Document.  This is the number of bytes
        /// per standard sector as a power of 2 (e.g. setting this to 9 sets the
        /// BytesPerSector to 2 ^ 9 or 512).  This is not normally changed from its
        /// default of 9.
        /// </summary>
        public ushort SectorSize
        {
            get { return _sectorSize; }
            set
            {
                if (value < 7)
                    throw new ArgumentOutOfRangeException("value", "must be >= 7");

                _sectorSize = value;
                _bytesPerSector = (int)Math.Pow(2, value);
            }
        }

        /// <summary>
        /// Gets or sets the Short Sector Size of this Ole2Document.  This is the number of 
        /// bytes per short sector as a power of 2 (e.g. setting this to 6 sets the
        /// BytesPerShortSector to 2 ^ 6 or 128).  This is not normally changed from
        /// its default of 6.
        /// </summary>
        public ushort ShortSectorSize
        {
            get { return _shortSectorSize; }
            set
            {
                if (value > SectorSize)
                    throw new ArgumentOutOfRangeException(string.Format("value must be <= SectorSize {0}", SectorSize));

                _shortSectorSize = value;
                _bytesPerShortSector = (int)Math.Pow(2, ShortSectorSize);
            }
        }

        private void SetDefaults()
        {
            //Standard from OOo compdocfileformat.pdf
            DocFileID = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };

            //Can be any 16-byte value per OOo compdocfileformat.pdf
            DocUID = new byte[16];

            //Unused standard per OOo compdocfileformat.pdf
            FileFormatRevision = new byte[] { 0x3E, 0x00 };

            //Unused standard per OOo compdocfileformat.pdf
            FileFormatVersion = new byte[] { 0x03, 0x00 };

            //Standard per OOo compdocfileformat
            IsLittleEndian = true;

            //Most common (BIFF8) value per OOo compdocfileformat.pdf
            SectorSize = 9;

            //Most common (BIFF8) value per OOo compdocfileformat.pdf
            ShortSectorSize = 6;

            //Unused per OOo compdocfileformat.pdf
            _blank1 = new byte[10];

            //Unused per OOo compdocfileformat.pdf
            _blank2 = new byte[4];

            //Most common (BIFF8) value per OOo compdocfileformat
            StandardStreamMinBytes = 4096;
        }

        /// <summary>
        /// Gets a Bytes object containing all the bytes of this Ole2Document.  This Bytes object's
        /// ByteArray is what should be saved to disk to persist this Ole2Document as a file on disk.
        /// </summary>
        public Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                //===================================================
                //DON'T CHANGE THE ORDER HERE OR ALL SID0'S WILL BE OFF
                //TODO: Should refactor this to function which does
                //lookups via a section-order array (would have to
                //check this for determining SID0's)
                //===================================================
                bytes.Append(Header.Bytes);
                bytes.Append(MSAT.Bytes);
                bytes.Append(SAT.Bytes);
                bytes.Append(SSAT.Bytes);
                bytes.Append(Streams.Bytes);
                bytes.Append(Directory.Bytes);

                return bytes;
            }
        }

        /// <summary>
        /// Loads this Ole2Document object from the provided stream (e.g. a FileStream to load
        /// from a File).  This is only preliminarily supported and tested for Excel
        /// files.
        /// </summary>
        /// <param name="stream">Stream to load the document from.</param>
        public void Load(System.IO.Stream stream)
        {
            if (stream.Length == 0)
                throw new Exception("No data (or zero-length) found!");

            if (stream.Length < 512)
                throw new Exception(string.Format("File length {0} < 512 bytes", stream.Length));

            byte[] head = new byte[512];
            stream.Read(head, 0, 512);

            bool isLE = false;
            if (head[28] == 254 && head[29] == 255)
                isLE = true;

            if (!isLE)
                throw new NotSupportedException("File is not Little-Endian");
            _isLittleEndian = isLE;

            ushort sectorSize = BitConverter.ToUInt16(MidByteArray(head, 30, 2), 0);
            if (sectorSize < 7 || sectorSize > 32)
                throw new Exception(string.Format("Invalid Sector Size [{0}] (should be 7 <= sectorSize <= 32", sectorSize));
            _sectorSize = sectorSize;

            ushort shortSectorSize = BitConverter.ToUInt16(MidByteArray(head, 32, 2), 0);
            if (shortSectorSize > sectorSize)
                throw new Exception(
                    string.Format("Invalid Short Sector Size [{0}] (should be < sectorSize; {1})", shortSectorSize, sectorSize));
            _shortSectorSize = shortSectorSize;

            //if (readError) ExitFunction;

            uint satSectorCount = BitConverter.ToUInt32(MidByteArray(head, 44, 4), 0);
            if (satSectorCount < 0)
                throw new Exception(string.Format("Invalid SAT Sector Count [{0}] (should be > 0)", satSectorCount));

            int dirSID0 = BitConverter.ToInt32(MidByteArray(head, 48, 4), 0);
            if (dirSID0 < 0)
                throw new Exception(string.Format("Invalid Directory SID0 [{0}] (should be > 0)", dirSID0));

            uint minStandardStreamSize = BitConverter.ToUInt32(MidByteArray(head, 56, 4), 0);
            if ((minStandardStreamSize < (Math.Pow(2, sectorSize))) || (minStandardStreamSize % (Math.Pow(2, sectorSize)) > 0))
                throw new Exception(string.Format("Invalid MinStdStreamSize [{0}] (should be multiple of (2^SectorSize)", minStandardStreamSize));
            _standardStreamMinBytes = minStandardStreamSize;

            int ssatSID0 = BitConverter.ToInt32(MidByteArray(head, 60, 4), 0);
            uint ssatSectorCount = BitConverter.ToUInt32(MidByteArray(head, 64, 4), 0);
            if (ssatSID0 < 0 && ssatSID0 != -2)
                throw new Exception(string.Format("Invalid SSAT SID0 [{0}] (must be >=0 or -2", ssatSID0));
            if (ssatSectorCount > 0 && ssatSID0 < 0)
                throw new Exception(
                    string.Format("Invalid SSAT SID0 [{0}] (must be >=0 when SSAT Sector Count > 0)", ssatSID0));
            if (ssatSectorCount < 0)
                throw new Exception(string.Format("Invalid SSAT Sector Count [{0}] (must be >= 0)", ssatSectorCount));

            int msatSID0 = BitConverter.ToInt32(MidByteArray(head, 68, 4), 0);
            if (msatSID0 < 1 && msatSID0 != -2)
                throw new Exception(string.Format("Invalid MSAT SID0 [{0}]", msatSID0));

            uint msatSectorCount = BitConverter.ToUInt32(MidByteArray(head, 72, 4), 0);
            if (msatSectorCount < 0)
                throw new Exception(string.Format("Invalid MSAT Sector Count [{0}]", msatSectorCount));
            else if (msatSectorCount == 0 && msatSID0 != -2)
                throw new Exception(string.Format("Invalid MSAT SID0 [{0}] (should be -2)", msatSID0));

            int i = 0;
            int k = ((int)Math.Pow(2, sectorSize) / 4) - 1;
            int[] msat = new int[108 + (k * msatSectorCount) + 1]; //add 1 compared to VBScript version due to C#/VBS array declaration diff
            for (int j = 0; j < 109; j++)
                msat[j] = BitConverter.ToInt32(MidByteArray(head, 76 + (j * 4), 4), 0);
            int msatSidNext = msatSID0;
            while (i < msatSectorCount)
            {
                Bytes sector = GetSector(stream, sectorSize, msatSidNext);
                if (sector.Length == 0)
                    throw new Exception(string.Format("MSAT SID Chain broken - SID [{0}] not found / EOF reached", msatSidNext));
                for (int j = 0; j < k; j++)
                    msat[109 + (i * k) + j] = BitConverter.ToInt32(sector.Get(j * 4, 4).ByteArray, 0);
                msatSidNext = BitConverter.ToInt32(sector.Get(k * 4, 4).ByteArray, 0);
                i++;
            }

            //if (re) Exit Function;

            //Find number of Sectors in SAT --> i
            i = msat.Length;
            while (msat[i - 1] < 0)
                i--;

            //Size and fill SAT SID array
            int[] sat = new int[(uint)(i * (Math.Pow(2, sectorSize) / 4))];
            int m = (int)(Math.Pow(2, sectorSize) / 4);
            for (int j = 0; j < i; j++)
            {
                Bytes sector = GetSector(stream, sectorSize, msat[j]);
                if (sector.Length == 0)
                    throw new Exception(string.Format("SAT SID Chain broken - SAT Sector SID{0} not found / EOF reached", msat[j]));
                for (k = 0; k < m; k++)
                    sat[(j * m) + k] = BitConverter.ToInt32(sector.Get(k * 4, 4).ByteArray, 0);
            }

            //Size and fill SSAT SID array
            i = 0;
            int ssatSidNext = ssatSID0;
            //		    m = (int) (Math.Pow(2, sectorSize) / 4);
            //Dictionary<int, int> ssat = new Dictionary<int, int>();
            int[] ssat = new int[(ssatSectorCount + 1) * m];
            while (ssatSidNext > -2)
            {
                Bytes sector = GetSector(stream, sectorSize, ssatSidNext);
                if (sector.Length == 0)
                    throw new Exception(string.Format("SSAT Sector SID{0} not found", ssatSidNext));
                for (int j = 0; j < m; j++)
                    ssat[(i * m) + j] = BitConverter.ToInt32(sector.Get(j * 4, 4).ByteArray, 0);
                ssatSidNext = sat[ssatSidNext];
                i++;
            }
            if (i < ssatSectorCount)
                throw new Exception(string.Format("SSAT Sector chain broken: {0} found, header indicates {1}", i, ssatSectorCount));

            //Size and fill Directory byte array array
            int dirSectorCount = 0;
            int dirSidNext = dirSID0;
            m = (int)(Math.Pow(2, sectorSize) / 128);
            Dictionary<int, byte[]> dir = new Dictionary<int, byte[]>();
            while (dirSidNext > -2)
            {
                Bytes sector = GetSector(stream, sectorSize, dirSidNext);
                if (sector.Length == 0)
                    throw new Exception(string.Format("Directory Sector SID{0} not found", dirSidNext));
                for (int j = 0; j < m; j++)
                    dir[(dirSectorCount * m) + j] = sector.Get(j * 128, 128).ByteArray;
                dirSidNext = sat[dirSidNext];
                dirSectorCount++;
            }

            for (i = 0; i < dir.Count; i++)
            {
                byte[] dirEntry = dir[i];
                int nameLength = BitConverter.ToInt16(MidByteArray(dirEntry, 64, 2), 0);
                byte[] docStreamName = MidByteArray(dirEntry, 0, nameLength);
                bool overwrite = false;
                if (Bytes.AreEqual(docStreamName, Directory.RootName))
                    overwrite = true;

                Bytes docStream = new Bytes();
                try
                {
                    //TODO WH
                    docStream = GetStream(stream, i, dir, sectorSize, sat, shortSectorSize, ssat, minStandardStreamSize);
                }
                catch (Exception ex)
                {
                }
                if (docStreamName.Length == 0 && docStream.Length == 0)
                    continue; //don't add streams for directory padding entries
                Streams.AddNamed(docStream, docStreamName, overwrite);
            }

        }

        private Bytes GetStream(System.IO.Stream fromDocumentStream, int did, Dictionary<int, byte[]> dir, ushort sectorSize, int[] sat, ushort shortSectorSize, int[] ssat, uint minStandardStreamSize)
        {
            Bytes stream = new Bytes();
            Bytes fromBytes;
            int[] fromSAT;
            ushort fromSectorSize;
            int sidNext;
            string shortness;

            int streamLength = BitConverter.ToInt32(MidByteArray(dir[did], 120, 4), 0);

            Bytes streamBytes = null;

            if (did == 0 || (streamLength >= minStandardStreamSize))
            {
                byte[] streamByteArray;
                streamByteArray = new byte[fromDocumentStream.Length];
                fromDocumentStream.Position = 0;
                fromDocumentStream.Read(streamByteArray, 0, streamByteArray.Length);
                streamBytes = new Bytes(streamByteArray);
            }

            if (did == 0)
            {
                fromSectorSize = sectorSize;
                fromSAT = sat;
                shortness = string.Empty;
                fromBytes = streamBytes;
            }
            else if (streamLength < minStandardStreamSize)
            {
                fromSectorSize = shortSectorSize;
                fromSAT = ssat;
                shortness = "Short ";
                fromBytes = GetStream(fromDocumentStream, 0, dir, sectorSize, sat, shortSectorSize, ssat, minStandardStreamSize);
            }
            else
            {
                fromSectorSize = sectorSize;
                fromSAT = sat;
                shortness = string.Empty;
                fromBytes = streamBytes;
            }

            sidNext = BitConverter.ToInt32(MidByteArray(dir[did], 116, 4), 0);
            while (sidNext > -2)
            {
                Bytes sector;
                if (did > 0 && streamLength < minStandardStreamSize)
                    sector = GetShortSectorBytes(fromBytes, fromSectorSize, sidNext);
                else
                    sector = GetSectorBytes(fromBytes, fromSectorSize, sidNext);

                if (sector.Length == 0)
                    throw new Exception(string.Format("{0}Sector not found [SID{1}]", shortness, sidNext));

                stream.Append(sector);

                sidNext = fromSAT[sidNext];
            }

            return stream.Get(streamLength);
        }

        private static Bytes GetSectorBytes(Bytes fromStream, int sectorSize, int sid)
        {
            int i = (int)Math.Pow(2, sectorSize);

            if (fromStream.Length < (sid * i))
                throw new Exception(string.Format("Invalid SID [{0}] (EOF reached)", sid));

            return fromStream.Get(512 + (sid * i), i);
        }

        private static Bytes GetShortSectorBytes(Bytes fromShortSectorStream, int shortSectorSize, int sid)
        {
            int i = (int)Math.Pow(2, shortSectorSize);

            if (fromShortSectorStream.Length < (sid * i))
                throw new Exception(string.Format("Invalid SID [{0}] (EOF reached)", sid));

            //	        return fromShortSectorStream.Get((sid * i) + 1, i);
            return fromShortSectorStream.Get(sid * i, i);
        }

        private static Bytes GetSector(System.IO.Stream stream, int sectorSize, int sidIndex)
        {
            int sectorLength = (int)Math.Pow(2, sectorSize);
            int offset = 512 + (sidIndex * sectorLength);
            if (stream.Length < (offset + sectorLength))
                return new Bytes();
            byte[] sector = new byte[sectorLength];
            stream.Seek(offset, SeekOrigin.Begin);
            //            stream.Position = offset;
            ReadWholeArray(stream, sector);
            //            int read = stream.Read(sector, 0, sectorLength);
            return new Bytes(sector);
        }

        /// <summary>
        /// Reads data into a complete array, throwing an EndOfStreamException
        /// if the stream runs out of data first, or if an IOException
        /// naturally occurs.
        /// http://www.yoda.arachsys.com/csharp/readbinary.html
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="data">The array to read bytes into. The array
        /// will be completely filled from the stream, so an appropriate
        /// size must be given.</param>
        public static void ReadWholeArray(System.IO.Stream stream, byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
        }

        private static byte[] MidByteArray(byte[] byteArray, int offset, int length)
        {
            if (offset >= byteArray.Length)
                throw new ArgumentOutOfRangeException(string.Format("offset {0} must be less than byteArray.Length {1}", offset, byteArray.Length));

            if (offset + length > byteArray.Length)
                throw new ArgumentOutOfRangeException(string.Format("offset {0} + length {1} must be <= byteArray.Length {2}", offset, length, byteArray.Length));

            if (offset == 0 && length == byteArray.Length)
                return byteArray;

            byte[] subArray = new byte[length];
            for (int i = 0; i < length; i++)
                subArray[i] = byteArray[offset + i];
            return subArray;
        }

        /// <summary>
        /// Gets a blank array of bytes.  Included as documented filler field.
        /// </summary>
        public byte[] Blank1
        {
            get { return _blank1; }
        }

        /// <summary>
        /// Gets a blank array of bytes.  Included as documented filler field.
        /// </summary>
        public byte[] Blank2
        {
            get { return _blank2; }
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Represents the SAT (Sector Allocation Table) for an OLE2 Document.
    /// </summary>
    public class Sat
    {
        private readonly Ole2Document _doc;

        /// <summary>
        /// Initializes a new instance of the Sat class for the provided Doc object.
        /// </summary>
        /// <param name="doc">The parent Doc object for this new Sat object.</param>
        public Sat(Ole2Document doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Gets a count of the sectors required by the body of this Sat.
        /// </summary>
        public int SectorCount
        {
            get
            {
                int sectorCount;

                int count =
                        _doc.SSAT.SectorCount
                        + _doc.Streams.ShortSectorStorage.SectorCount
                        + _doc.Streams.SectorCount
                        + _doc.Directory.SectorCount;

                int sidsPerSector = _doc.BytesPerSector / 4;
                sectorCount = (int)Math.Ceiling((count + Math.Ceiling((double)count / sidsPerSector)) / sidsPerSector);

                return sectorCount;
            }
        }

        /// <summary>
        /// Gets the SID0, or SID of the first sector, of this Sat.
        /// </summary>
        public int SID0
        {
            get
            {
                if (_doc.MSAT.SID0 == -2)
                    return 0;
                else
                    return _doc.MSAT.SectorCount;
            }
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                int i, j, k, m, sid, sectorCount;

                sid = 0;

                sectorCount = _doc.MSAT.SectorCount;
                j = (sid + (sectorCount - 1));
                for (i = sid; i <= j; i++)
                    bytes.Append(BitConverter.GetBytes((int)-4));
                sid = i;

                sectorCount = _doc.SAT.SectorCount;
                j = (sid + (sectorCount - 1));
                for (i = sid; i <= j; i++)
                    bytes.Append(BitConverter.GetBytes((int)-3));
                sid = i;

                sectorCount = _doc.SSAT.SectorCount;
                j = (sid + (sectorCount - 1));
                for (i = sid; i <= j; i++)
                {
                    if (i < j)
                        bytes.Append(BitConverter.GetBytes((int)(i + 1)));
                    else
                        bytes.Append(BitConverter.GetBytes((int)-2));
                }
                sid = i;

                sectorCount = _doc.Streams.ShortSectorStorage.SectorCount;
                j = (sid + (sectorCount - 1));
                for (i = sid; i <= j; i++)
                {
                    if (i < j)
                        bytes.Append(BitConverter.GetBytes((int)(i + 1)));
                    else
                        bytes.Append(BitConverter.GetBytes((int)-2));
                }
                sid = i;

                m = _doc.Streams.Count;
                for (k = 1; k <= m; k++)
                {
                    sectorCount = _doc.Streams[k].SectorCount;
                    j = (sid + (sectorCount - 1));
                    for (i = sid; i <= j; i++)
                    {
                        if (i < j)
                            bytes.Append(BitConverter.GetBytes((int)(i + 1)));
                        else
                            bytes.Append(BitConverter.GetBytes((int)-2));
                    }
                    sid = i;
                }

                sectorCount = _doc.Directory.SectorCount;
                j = (sid + (sectorCount - 1));
                for (i = sid; i <= j; i++)
                {
                    if (i < j)
                        bytes.Append(BitConverter.GetBytes((int)(i + 1)));
                    else
                        bytes.Append(BitConverter.GetBytes((int)-2));
                }
                sid = i;

                int remainingSlots = (int)(((decimal)sid) % ((decimal)_doc.BytesPerSector / 4));
                //                if (remainingSlots != 0)
                //                {
                j = (_doc.BytesPerSector / 4) - remainingSlots;
                for (i = 1; i <= j; i++)
                    bytes.Append(BitConverter.GetBytes((int)-1));
                //                }

                return bytes;
            }
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Represents the SSAT (Short Sector Allocation Table) of an OLE2 Document.
    /// </summary>
    public class Ssat
    {
        private readonly Ole2Document _doc;

        /// <summary>
        /// Initializes a new instance of the Ssat class for the provided Doc object.
        /// </summary>
        /// <param name="doc">The parent Doc object for this new Ssat object.</param>
        public Ssat(Ole2Document doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Gets a count of the sectors required for this Ssat in the Doc.
        /// </summary>
        public int SectorCount
        {
            get
            {
                int sectorCount;
                int i, j, count;

                count = 0;
                j = _doc.Streams.Count;
                for (i = 1; i <= j; i++)
                    count += _doc.Streams[i].ShortSectorCount;

                sectorCount = (int)Math.Ceiling(count / (((decimal)_doc.BytesPerSector) / 4));

                return sectorCount;
            }
        }

        /// <summary>
        /// Gets the SID0, or SID of the first sector, of this Ssat.
        /// </summary>
        public int SID0
        {
            get
            {
                if (SectorCount > 0)
                    return _doc.SAT.SID0 + _doc.SAT.SectorCount;
                else
                    return -2;
            }
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();
                int sidCount;
                Stream stream;

                sidCount = 0;
                int streamCount = _doc.Streams.Count;
                for (int i = 1; i <= streamCount; i++)
                {
                    stream = _doc.Streams[i];
                    int shortSectorCount = stream.ShortSectorCount;
                    sidCount += shortSectorCount;
                    int sid0 = stream.SID0;
                    for (int k = 1; k <= shortSectorCount; k++)
                    {
                        if (k < shortSectorCount)
                            bytes.Append(BitConverter.GetBytes((int)(sid0 + k)));
                        else
                            bytes.Append(BitConverter.GetBytes((int)-2));
                    }
                }

                if (sidCount > 0)
                {
                    //j = (int) Math.Floor((decimal) _doc.BytesPerSector/4);
                    streamCount = (_doc.BytesPerSector / 4) - (sidCount % (_doc.BytesPerSector / 4));
                    for (int i = 1; i <= streamCount; i++)
                        bytes.Append(BitConverter.GetBytes(-1));
                }

                return bytes;
            }
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Represents a Stream (either standard or short) within an OLE2 Document.  This is the basic
    /// unit of storage within the OLE2 Compound Document format.
    /// </summary>
    public class Stream
    {
        private readonly Ole2Document _doc;
        private byte[] _name = new byte[0];
        private Bytes _bytes = new Bytes();

        /// <summary>
        /// Initializes a new instance of the Stream class for the provided Doc object.
        /// </summary>
        /// <param name="doc">The parent Doc object for this new Stream object.</param>
        public Stream(Ole2Document doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Gets whether this Stream is a short stream (is less than Doc.StandardStreamMinBytes
        /// in length) or a standard stream.
        /// </summary>
        public bool IsShort
        {
            get
            {
                if (Name == Directory.RootName)
                    return false;
                else
                {
                    if (_bytes.Length < _doc.StandardStreamMinBytes)
                        return true;
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets a Bytes object containing all the bytes of this Stream (not padded to standard or
        /// sector length).
        /// </summary>
        public Bytes Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }

        /// <summary>
        /// Gets or sets the byte[] representing the Name of this Stream.
        /// </summary>
        public byte[] Name
        {
            get { return _name; }
            set { if (value == null) throw new ArgumentNullException(); _name = value; }
        }

        /// <summary>
        /// Gets a count of the bytes contained in this Stream.
        /// </summary>
        public int ByteCount
        {
            get
            {
                int count;

                count = 0;
                if (Bytes.AreEqual(Name, Directory.RootName))
                {
                    int bytesPerShortSector = _doc.BytesPerShortSector;
                    int streamCount = _doc.Streams.Count;
                    for (int i = 1; i <= streamCount; i++)
                        count += (_doc.Streams[i].ShortSectorCount * bytesPerShortSector);
                }
                else
                    count = _bytes.Length;

                return count;
            }
        }

        /// <summary>
        /// Gets a count of the sectors (standard only) required by this stream.
        /// </summary>
        public int SectorCount
        {
            get
            {
                if (!IsShort)
                    return GetSectorCount();
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets a count of the sectors (short only) required by this stream.
        /// </summary>
        public int ShortSectorCount
        {
            get
            {
                if (IsShort)
                    return GetSectorCount();
                else
                    return 0;
            }
        }

        private int GetSectorCount()
        {
            int bytesPerSector;
            decimal decimalSectors;

            if (IsShort)
                bytesPerSector = _doc.BytesPerShortSector;
            else
                bytesPerSector = _doc.BytesPerSector;

            decimalSectors = ((decimal)ByteCount) / bytesPerSector;
            return (int)Math.Ceiling(decimalSectors);
        }

        /// <summary>
        /// Gets the SID0, or SID of the first sector, of this Stream.
        /// </summary>
        public int SID0
        {
            get
            {
                int sid0;
                Stream stream;

                if (Bytes.AreEqual(Name, Directory.RootName))
                    return _doc.SSAT.SID0 + _doc.SSAT.SectorCount;

                int j = _doc.Streams.GetIndex(Name) - 1;
                if (IsShort)
                {
                    sid0 = 0;
                    for (int i = 1; i <= j; i++)
                    {
                        stream = _doc.Streams[i];
                        if (stream.IsShort)
                            sid0 += stream.ShortSectorCount;
                    }
                }
                else
                {
                    sid0 = _doc.SSAT.SID0;
                    if (sid0 == -2)
                        sid0 = _doc.SAT.SID0 + _doc.SAT.SectorCount;
                    else
                        sid0 += _doc.SSAT.SectorCount;
                    sid0 += _doc.Streams.ShortSectorStorage.SectorCount;
                    for (int i = 1; i <= j; i++)
                    {
                        stream = _doc.Streams[i];
                        if (!stream.IsShort)
                            sid0 += stream.SectorCount;
                    }
                }

                return sid0;
            }
        }
    }
}
namespace Core.MyOle2
{
    /// <summary>
    /// Represents and manages the collection of Streams for a given OLE2 Document.
    /// </summary>
    public class Streams
    {
        private readonly Ole2Document _doc;
        private readonly List<Stream> _streams = new List<Stream>();

        /// <summary>
        /// Initializes a new instance of the Streams class for the given Doc object.
        /// </summary>
        /// <param name="doc">The parent Doc object for this new Streams object.</param>
        public Streams(Ole2Document doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Adds a new Stream with the given name and containing the given bytes 
        /// to this Streams collection.  If a stream by the name exists, an exception
        /// will be thrown.
        /// </summary>
        /// <param name="bytes">The byte[] to be contained by the new Stream.</param>
        /// <param name="name">The byte[] of the new Stream's name.</param>
        /// <returns>The new Stream object in this Streams collection with the given
        /// name and bytes.</returns>
        public Stream AddNamed(byte[] bytes, byte[] name)
        {
            return AddNamed(new Bytes(bytes), name);
        }

        /// <summary>
        /// Adds a new Stream with the given name and containing the given bytes
        /// to this Streams collection.  If a stream by the name already exists,
        /// an exception will be thrown.
        /// </summary>
        /// <param name="bytes">A Bytes object containing the bytes for the new
        /// Stream.</param>
        /// <param name="name">A Bytes object containing the bytes for the name
        /// of the new Stream.</param>
        /// <returns>The new Stream object in this Streams collection with the given
        /// name and bytes.</returns>
        public Stream AddNamed(Bytes bytes, byte[] name)
        {
            return AddNamed(bytes, name, false);
        }

        /// <summary>
        /// Adds a new Stream with the given name and containing the given bytes
        /// to this Streams collection.  If a stream by the name already exists,
        /// an exception will be thrown or the stream's bytes will be overwritten,
        /// depending on the value in the overwrite parameter.
        /// </summary>
        /// <param name="bytes">A Bytes object containing the bytes for the new
        /// Stream.</param>
        /// <param name="name">A Bytes object containing the bytes for the name
        /// of the new Stream.</param>
        /// <param name="overwrite">Determines the behaviour (overwriting the bytes
        /// or throwing an exception) if a stream by the provided name already
        /// exists.</param>
        /// <returns>The new Stream object in this Streams collection with the given
        /// name and bytes.</returns>
        public Stream AddNamed(Bytes bytes, byte[] name, bool overwrite)
        {
            Stream stream;
            int streamIndex = GetIndex(name);
            if (streamIndex != -1 && !overwrite)
                throw new ArgumentException("value already exists", "name");
            else if (streamIndex != -1)
                stream = this[streamIndex];
            else
            {
                stream = new Stream(_doc);
                stream.Name = name;
                _streams.Add(stream);
            }

            stream.Bytes = bytes;

            return stream;
        }

        /// <summary>
        /// Gets a count of the number of streams included in or managed by this Streams
        /// collection (or the number of streams in this Doc).
        /// </summary>
        public int Count
        {
            get { return _streams.Count - 1; } //Don't count the Short Sector Storage Stream
        }

        /// <summary>
        /// Gets a count of the sectors required by all the streams in this Streams collection
        /// (or in this Doc).
        /// </summary>
        public int SectorCount
        {
            get
            {
                int sectorCount = 0;

                sectorCount += ShortSectorStorage.SectorCount;
                for (int i = 1; i <= Count; i++)
                    sectorCount += this[i].SectorCount;

                return sectorCount;
            }
        }

        /// <summary>
        /// Gets the Short Sector Storage stream for this Doc.
        /// </summary>
        public Stream ShortSectorStorage
        {
            get { return _streams[0]; }
        }

        /// <summary>
        /// Gets the Stream with the given index.
        /// </summary>
        /// <param name="idx">The int index or byte[] name of the Stream to get.</param>
        /// <returns>The Stream object of the given int index or byte[] name.</returns>
        public Stream this[object idx]
        {
            get
            {
                if (idx is int)
                {
                    return _streams[(int)idx];
                }
                else if (idx is byte[])
                {
                    return _streams[GetIndex((byte[])idx)];
                }
                else if (idx is string) //TODO: Should probably exclude Root Storage
                {
                    throw new NotImplementedException();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the int index of the Stream with the provided name.
        /// </summary>
        /// <param name="streamName">The byte[] name of the Stream to get the index of.</param>
        /// <returns>The int index of the Stream with the given byte[] name; else -1.</returns>
        public int GetIndex(byte[] streamName)
        {
            for (int i = 0; i < _streams.Count; i++)
            {
                if (Bytes.AreEqual(_streams[i].Name, streamName))
                    return i;
            }

            return -1;
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                int lastLen;
                Stream stream;

                for (int i = 1; i <= Count; i++)
                {
                    stream = this[i];
                    if (stream.IsShort)
                    {
                        bytes.Append(stream.Bytes);
                        lastLen = (int)((decimal)stream.Bytes.Length % _doc.BytesPerShortSector);
                        if (lastLen > 0)
                            bytes.Append(new byte[_doc.BytesPerShortSector - lastLen]);
                    }
                }
                lastLen = (int)((decimal)bytes.Length % _doc.BytesPerSector);
                if (lastLen > 0)
                    bytes.Append(new byte[_doc.BytesPerSector - lastLen]);

                for (int i = 1; i <= Count; i++)
                {
                    stream = this[i];
                    if (!stream.IsShort)
                    {
                        bytes.Append(stream.Bytes);
                        lastLen = (int)((decimal)stream.Bytes.Length % _doc.BytesPerSector);
                        if (lastLen > 0)
                            bytes.Append(new byte[_doc.BytesPerSector - lastLen]);
                    }
                }

                return bytes;
            }
        }
    }
}
namespace Core.MyOle2.Metadata
{
    /// <summary>
    /// Represents and presents functionality for managing an OLE2 DocumentSummaryInformation
    /// Metadata Stream.
    /// </summary>
    public class DocumentSummaryInformationSection : MetadataStream.Section
    {
        private static readonly byte[] FORMAT_ID_SECTION_0 = new byte[] {
                0x02, 0xD5, 0xCD, 0xD5, 0x9C, 0x2E, 0x1B, 0x10, 0x93, 0x97, 0x08, 0x00, 0x2B, 0x2C, 0xF9, 0xAE };

        private const uint ID_DICTIONARY = 0;
        private const uint ID_CODEPAGE = 1;
        private const uint ID_CATEGORY = 2;
        private const uint ID_PRESENTATION_TARGET = 3;
        private const uint ID_BYTES = 4;
        private const uint ID_LINES = 5;
        private const uint ID_PARAGRAPHS = 6;
        private const uint ID_SLIDES = 7;
        private const uint ID_NOTES = 8;
        private const uint ID_HIDDEN_SLIDES = 9;
        private const uint ID_MM_CLIPS = 10;
        private const uint ID_SCALE_CROP = 11;
        private const uint ID_HEADING_PAIRS = 12;
        private const uint ID_TITLES_OF_PARTS = 13;
        private const uint ID_MANAGER = 14;
        private const uint ID_COMPANY = 15;
        private const uint ID_LINKS_UP_TO_DATE = 16;

        /// <summary>
        /// Initializes a new instance of the DocumentSummaryInformation class.
        /// </summary>
        public DocumentSummaryInformationSection()
        {
            FormatId = FORMAT_ID_SECTION_0;
            CodePage = 1252;
        }

        //TODO: Implement OLE2 Summary Stream Dictionary object
        //        public object Dictionary
        //        {
        //            get { return _dictionary; }
        //            set { _dictionary = value; }
        //        }

        /// <summary>
        /// Gets or sets the CodePage of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public short? CodePage
        {
            get { return (short?)GetProperty(ID_CODEPAGE); }
            set { SetProperty(ID_CODEPAGE, Property.Types.VT_I2, value); }
        }

        /// <summary>
        /// Gets or sets the Category of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Category
        {
            get { return (string)GetProperty(ID_CATEGORY); }
            set { SetProperty(ID_CATEGORY, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the PresentationTarget of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string PresentationTarget
        {
            get { return (string)GetProperty(ID_PRESENTATION_TARGET); }
            set { SetProperty(ID_PRESENTATION_TARGET, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Bytes Property of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? BytesProperty
        {
            get { return (int?)GetProperty(ID_BYTES); }
            set { SetProperty(ID_BYTES, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the Lines of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? Lines
        {
            get { return (int?)GetProperty(ID_LINES); }
            set { SetProperty(ID_LINES, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the Paragraphs of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? Paragraphs
        {
            get { return (int?)GetProperty(ID_PARAGRAPHS); }
            set { SetProperty(ID_PARAGRAPHS, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the Slides of ths DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? Slides
        {
            get { return (int?)GetProperty(ID_SLIDES); }
            set { SetProperty(ID_SLIDES, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the Notes of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? Notes
        {
            get { return (int?)GetProperty(ID_NOTES); }
            set { SetProperty(ID_NOTES, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the HiddenSlides of this DocumentSummaryInformation Section.  Setting to null removes property.
        /// </summary>
        public int? HiddenSlides
        {
            get { return (int?)GetProperty(ID_HIDDEN_SLIDES); }
            set { SetProperty(ID_HIDDEN_SLIDES, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the MM Clips of this DocumentSummaryInformation Section.  Setting to null removes property.
        /// </summary>
        public int? MmClips
        {
            get { return (int?)GetProperty(ID_MM_CLIPS); }
            set { SetProperty(ID_MM_CLIPS, Property.Types.VT_I4, value); }
        }

        //        public bool? ScaleCrop
        //        {
        //            get { return (bool?)GetProperty(ID_SCALE_CROP); }
        //            set { SetProperty(ID_SCALE_CROP, Property.Types.VT_BOOL, value); }
        //        }

        //        public object HeadingPairs
        //        {
        //            get { return _headingPairs; }
        //            set { _headingPairs = value; }
        //        }

        //        public object TitlesOfParts
        //        {
        //            get { return _titlesOfParts; }
        //            set { _titlesOfParts = value; }
        //        }

        /// <summary>
        /// Gets or sets the Manager of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Manager
        {
            get { return (string)GetProperty(ID_MANAGER); }
            set { SetProperty(ID_MANAGER, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Company of this DocumentSummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Company
        {
            get { return (string)GetProperty(ID_COMPANY); }
            set { SetProperty(ID_COMPANY, Property.Types.VT_LPSTR, value); }
        }

        //        public bool? LinksUpToDate
        //        {
        //            get { return _linksUpToDate; }
        //            set { _linksUpToDate = value; }
        //        }
    }
}
namespace Core.MyOle2.Metadata
{
    public partial class MetadataStream
    {
        /// <summary>
        /// Represents and provides functionality for managing an OLE2 Metadata Stream's Header.
        /// </summary>
        public class Header
        {
            private OriginOperatingSystems _originOperatingSystem =
                OriginOperatingSystems.Default;

            private OriginOperatingSystemVersions _originOperatingSystemVersion =
                OriginOperatingSystemVersions.Default;

            private byte[] _classId = new byte[16];

            private MetadataStream _parent;

            internal Header(MetadataStream parent)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");

                _parent = parent;
            }

            /// <summary>
            /// Gets or sets the operating system on which this document was created.
            /// </summary>
            public OriginOperatingSystems OriginOperatingSystem
            {
                get { return _originOperatingSystem; }
                set { _originOperatingSystem = value; }
            }

            /// <summary>
            /// Gets or sets the version of the operating system on which this document was created.
            /// </summary>
            public OriginOperatingSystemVersions OriginOperatingSystemVersion
            {
                get { return _originOperatingSystemVersion; }
                set { _originOperatingSystemVersion = value; }
            }

            /// <summary>
            /// Gets or sets the ClassID related to this document.
            /// </summary>
            public byte[] ClassID
            {
                get { return _classId; }
                set { _classId = value; }
            }

            internal Bytes Bytes
            {
                get
                {
                    Bytes bytes = new Bytes();
                    bytes.Append(new byte[2] { 0xFE, 0xFF });
                    bytes.Append(new byte[2] { 0x00, 0x00 }); //listed separately for ease of reference to the documentation
                    bytes.Append(Metadata.OriginOperatingSystemVersion.GetBytes(_originOperatingSystemVersion));
                    bytes.Append(Metadata.OriginOperatingSystem.GetBytes(_originOperatingSystem));
                    bytes.Append(_classId);
                    bytes.Append(BitConverter.GetBytes(_parent.Sections.Count));
                    return bytes;
                }
            }
        }
    }
}
namespace Core.MyOle2.Metadata
{
    /// <summary>
    /// Provides base functionality to aid in representint an OLE2 Metadata Stream, 
    /// such as the SummaryInformation or DocumentSummaryInformation streams.  Implementation
    /// requirements were obtained from http://www.rainer-klute.de/~klute/Software/poibrowser/doc/HPSF-Description.html.
    /// </summary>
    public partial class MetadataStream
    {
        private Ole2Document _parentDocument;
        private Header _header;
        private SectionList _sectionList = new SectionList();

        /// <summary>
        /// Initializes a new instance of the MetadataStream class for the given parent
        /// Ole2Document.
        /// </summary>
        /// <param name="parentDocument">The parent Ole2Document for which to initialize
        /// this MetadataStream.</param>
        public MetadataStream(Ole2Document parentDocument)
        {
            _parentDocument = parentDocument;
            _header = new Header(this);
        }

        /// <summary>
        /// Gets the SectionList of this Metadata object.
        /// </summary>
        public SectionList Sections
        {
            get { return _sectionList; }
        }

        /// <summary>
        /// Gets a Bytes object containing all the bytes in this Metadata stream.
        /// </summary>
        public Bytes Bytes
        {
            get
            {
                Bytes bytes = new Bytes();

                bytes.Append(_header.Bytes);
                bytes.Append(_sectionList.Bytes);

                return bytes;
            }
        }
    }
}
namespace Core.MyOle2.Metadata
{
    ///<summary>
    /// Origin Operating System choices in OLE2 SummaryInformation streams.
    ///</summary>
    public enum OriginOperatingSystems
    {
        /// <summary>16-bit Windows</summary>
        Win16,

        /// <summary>Macintosh</summary>
        Macintosh,

        /// <summary>32-bit Windows</summary>
        Win32,

        /// <summary>Default - 32-bit Windows</summary>
        Default = Win32
    }

    internal static class OriginOperatingSystem
    {
        internal static byte[] GetBytes(OriginOperatingSystems system)
        {
            switch (system)
            {
                case OriginOperatingSystems.Win16:
                    return new byte[] { 0x00, 0x00 };
                case OriginOperatingSystems.Macintosh:
                    return new byte[] { 0x01, 0x00 };
                case OriginOperatingSystems.Win32:
                    return new byte[] { 0x02, 0x00 };
                default:
                    throw new ArgumentException(string.Format("unexpected value {0}", system.ToString()), "system");
            }
        }
    }
}
namespace Core.MyOle2.Metadata
{
    /// <summary>
    /// Versions available to describe the specified Operating System.
    /// </summary>
    public enum OriginOperatingSystemVersions
    {
        /// <summary>Default - this is the only known available value.</summary>
        Default
    }

    internal static class OriginOperatingSystemVersion
    {
        internal static byte[] GetBytes(OriginOperatingSystemVersions version)
        {
            return new byte[] { 0x05, 0x01 };
        }
    }
}
namespace Core.MyOle2.Metadata
{
    /// <summary>
    /// Represents a OLE2 Summary Information stream property.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// The different OLE2 Summary Information stream property types.
        /// </summary>
        public enum Types : uint
        {
            /// <summary>nothing</summary>
            VT_EMPTY = 0,
            /// <summary>SQL style Null</summary>
            VT_NULL = 1,
            /// <summary>2 byte signed int</summary>
            VT_I2 = 2,
            /// <summary>4 byte signed int</summary>
            VT_I4 = 3,
            /// <summary>4 byte real</summary>
            VT_R4 = 4,
            /// <summary>8 byte real</summary>
            VT_R8 = 5,
            /// <summary>Currency</summary>
            VT_CY = 6,
            /// <summary>Date</summary>
            VT_DATE = 7,
            /// <summary>OLE Automation string</summary>
            VT_BSTR = 8,
            /// <summary>*IDispatch</summary>
            VT_DISPATCH = 9,
            /// <summary>SCODE</summary>
            VT_ERROR = 10,
            /// <summary>Boolean (true=-1; false=0)</summary>
            VT_BOOL = 11,
            /// <summary>Variant*</summary>
            VT_VARIANT = 12,
            /// <summary>IUnknown*</summary>
            VT_UNKNOWN = 13,
            /// <summary>16 byte fixed point</summary>
            VT_DECIMAL = 14,
            /// <summary>signed char</summary>
            VT_I1 = 16,
            /// <summary>unsigned char</summary>
            VT_UI1 = 17,
            /// <summary>unsigned short</summary>
            VT_UI2 = 18,
            /// <summary>unsigned short</summary>
            VT_UI4 = 19,
            /// <summary>signed 64-bit int</summary>
            VT_I8 = 20,
            /// <summary>unsigned 64-bit int</summary>
            VT_UI8 = 21,
            /// <summary>signed machine int</summary>
            VT_INT = 22,
            /// <summary>unsigned machine int</summary>
            VT_UINT = 23,
            /// <summary>C style void</summary>
            VT_VOID = 24,
            /// <summary>Standard return type</summary>
            VT_HRESULT = 25,
            /// <summary>pointer type</summary>
            VT_PTR = 26,
            /// <summary>(use VT_ARRAY in VARIANT)</summary>
            VT_SAFEARRAY = 27,
            /// <summary>C style array</summary>
            VT_CARRAY = 28,
            /// <summary>user defined type</summary>
            VT_USERDEFINED = 29,
            /// <summary>null terminated string</summary>
            VT_LPSTR = 30,
            /// <summary>wide null terminated string</summary>
            VT_LPWSTR = 31,
            /// <summary>FILETIME</summary>
            VT_FILETIME = 64,
            /// <summary>length prefixed bytes</summary>
            VT_BLOB = 65,
            /// <summary>Name of the stream follows</summary>
            VT_STREAM = 66,
            /// <summary>Name of the storage follows</summary>
            VT_STORAGE = 67,
            /// <summary>Stream contains an object</summary>
            VT_STREAMED_OBJECT = 68,
            /// <summary>Storage contains an object</summary>
            VT_STORED_OBJECT = 69,
            /// <summary>Blob contains an object</summary>
            VT_BLOB_OBJECT = 70,
            /// <summary>Clipboard format</summary>
            VT_CF = 71,
            /// <summary>A Class ID</summary>
            VT_CLSID = 72,
            /// <summary>simple counted array</summary>
            VT_VECTOR,//0x1000
            /// <summary>SAFEARRAY*</summary>
            VT_ARRAY,//0x2000
            /// <summary>void* for local use</summary>
            VT_BYREF,//0x4000
            /// <summary></summary>
            VT_RESERVED,//0x8000
            /// <summary></summary>
            VT_ILLEGAL,//0xFFFF
            /// <summary></summary>
            VT_ILLEGALMASKED,//0xFFF
            /// <summary></summary>
            VT_TYPEMASK//0xFFF
        }

        private object _value;
        private Types _type;
        private uint _id;

        /// <summary>
        /// Initializes a new instance of the Property class with the given id, type and value.
        /// </summary>
        /// <param name="id">The id for the new Property.</param>
        /// <param name="type">The type of the new Property.</param>
        /// <param name="value">The value of the new Property.</param>
        public Property(uint id, Types type, object value)
        {
            _id = id;
            _type = type;
            _value = value;
        }

        /// <summary>
        /// Gets the Types value of this Property.
        /// </summary>
        public Types Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the Id of this Property.
        /// </summary>
        public uint Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets or sets the Value of this Property.
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        internal Bytes Bytes
        {
            get
            {
                if (_value == null)
                    throw new ApplicationException(string.Format("The Value of a Property can't be null - Property ID {0}", _id));

                Bytes bytes = new Bytes();

                bytes.Append(BitConverter.GetBytes((uint)_type));
                bytes.Append(GetBytes(this));

                return bytes;
            }
        }

        private static Bytes GetBytes(Property property)
        {
            Bytes bytes;
            switch (property.Type)
            {
                case Types.VT_LPSTR: bytes = GetBytesLPSTR(property.Value); break;
                case Types.VT_I2: bytes = GetBytesI2(property.Value); break;
                case Types.VT_I4: bytes = GetBytesI4(property.Value); break;
                case Types.VT_FILETIME: bytes = GetBytesFILETIME(property.Value); break;
                //case Types.VT_BOOL: bytes = GetBytesBOOL(property.Value); break;

                //TODO: Implement these as necessary
                case Types.VT_EMPTY:
                case Types.VT_NULL:
                case Types.VT_R4:
                case Types.VT_R8:
                case Types.VT_CY:
                case Types.VT_DATE:
                case Types.VT_BSTR:
                case Types.VT_DISPATCH:
                case Types.VT_ERROR:
                case Types.VT_BOOL:
                case Types.VT_VARIANT:
                case Types.VT_UNKNOWN:
                case Types.VT_DECIMAL:
                case Types.VT_I1:
                case Types.VT_UI1:
                case Types.VT_UI2:
                case Types.VT_UI4:
                case Types.VT_I8:
                case Types.VT_UI8:
                case Types.VT_INT:
                case Types.VT_UINT:
                case Types.VT_VOID:
                case Types.VT_HRESULT:
                case Types.VT_PTR:
                case Types.VT_SAFEARRAY:
                case Types.VT_CARRAY:
                case Types.VT_USERDEFINED:
                case Types.VT_LPWSTR:
                case Types.VT_BLOB:
                case Types.VT_STREAM:
                case Types.VT_STORAGE:
                case Types.VT_STREAMED_OBJECT:
                case Types.VT_STORED_OBJECT:
                case Types.VT_BLOB_OBJECT:
                case Types.VT_CF:
                case Types.VT_CLSID:
                case Types.VT_VECTOR:
                case Types.VT_ARRAY:
                case Types.VT_BYREF:
                case Types.VT_RESERVED:
                case Types.VT_ILLEGAL:
                case Types.VT_ILLEGALMASKED:
                case Types.VT_TYPEMASK:
                    throw new NotSupportedException(string.Format("Property Type {0}", property.Type));
                default:
                    throw new ApplicationException(string.Format("unexpected value {0}", property.Type));
            }

            //Documentation says it's padded to a multiple of 4;
            int partial4 = bytes.Length % 4;
            if (partial4 != 0)
                bytes.Append(new byte[4 - partial4]);

            return bytes;
        }

        private static Bytes GetBytesBOOL(object value)
        {
            bool theBool = (bool)value;
            int i = theBool ? -1 : 0;
            return new Bytes(BitConverter.GetBytes(i));
        }

        private static Bytes GetBytesFILETIME(object value)
        {
            DateTime theDate = (DateTime)value;
            return new Bytes(BitConverter.GetBytes(theDate.ToFileTime()));
        }

        private static Bytes GetBytesI4(object value)
        {
            int theInt = (int)value;
            return new Bytes(BitConverter.GetBytes(theInt));
        }

        private static Bytes GetBytesI2(object value)
        {
            short theShort = (short)value;
            return new Bytes(BitConverter.GetBytes(theShort));
        }

        private static Bytes GetBytesLPSTR(object value)
        {
            Bytes lpstr = new Bytes();

            string theString = value as string;
            Encoder encoder = Encoding.ASCII.GetEncoder();
            char[] theChars = theString.ToCharArray();
            int paddedLength = theChars.Length + 1; //add one for terminating null
            paddedLength += (paddedLength % 4); //must be multiple of 4
            byte[] bytes = new byte[paddedLength];
            encoder.GetBytes(theChars, 0, theChars.Length, bytes, 0, true);
            lpstr.Append(BitConverter.GetBytes((uint)paddedLength));
            lpstr.Append(bytes);

            return lpstr;
        }
    }
}
namespace Core.MyOle2.Metadata
{
    /// <summary>
    /// Represents a list of OLE2 Metadata Stream Properties.
    /// </summary>
    public class PropertyList
    {
        private Dictionary<uint, Property> _properties = new Dictionary<uint, Property>();

        internal PropertyList()
        {

        }

        /// <summary>
        /// Gets or sets the Property in this collection with the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Property this[uint id]
        {
            get { return _properties[id]; }
            set { Add(value, true); }
        }

        /// <summary>
        /// Gets the number of Properties contained in this collection.
        /// </summary>
        public uint Count
        {
            get { return (uint)_properties.Count; }
        }

        /// <summary>
        /// Adds a Property to this Collection, optionally overwriting any previously
        /// existent Property with the same id.
        /// </summary>
        /// <param name="property">The Property to add to this collection.</param>
        /// <param name="overwrite">true to allow overwriting any existing Property with the
        /// same id, false otherwise.</param>
        public void Add(Property property, bool overwrite)
        {
            if (!overwrite && _properties.ContainsKey(property.Id))
                throw new ApplicationException(string.Format("Can't overwrite existing property with id {0}", property.Id));
            _properties[property.Id] = property;
        }

        internal Bytes Bytes
        {
            get
            {
                Bytes propertiesBytes = new Bytes();
                Bytes propertyListBytes = new Bytes();
                int offset = 8 + (8 * _properties.Count);
                foreach (uint id in _properties.Keys)
                {
                    Property property = _properties[id];
                    Bytes propertyBytes = property.Bytes;
                    propertyListBytes.Append(BitConverter.GetBytes(property.Id));
                    propertyListBytes.Append(BitConverter.GetBytes((uint)offset));
                    offset += propertyBytes.Length;
                    propertiesBytes.Append(propertyBytes);
                }

                Bytes bytes = new Bytes();

                bytes.Append(propertyListBytes);
                bytes.Append(propertiesBytes);

                return bytes;
            }
        }

        /// <summary>
        /// Determines whether the PropertyList contains the specified key.
        /// </summary>
        /// <param name="id">The id of the Property to check for.</param>
        /// <returns>true if this PropertyList contains the key id, false otherwise.</returns>
        public bool ContainsKey(uint id)
        {
            return _properties.ContainsKey(id);
        }

        /// <summary>
        /// Removes the specified key from the PropertyList.
        /// </summary>
        /// <param name="id">The id of the Propert to remove.</param>
        /// <returns>true if the Property with the specified id was removed, false otherwise.</returns>
        public bool Remove(uint id)
        {
            return _properties.Remove(id);
        }
    }
}
namespace Core.MyOle2.Metadata
{
    public partial class MetadataStream
    {
        /// <summary>
        /// Represents a Section of a SummaryInformation stream.
        /// </summary>
        public abstract class Section
        {
            private byte[] _formatId = new byte[16];
            private PropertyList _properties = new PropertyList();

            internal Bytes Bytes
            {
                get
                {
                    Bytes propertyListBytes = Properties.Bytes;
                    uint length = (uint)(8 + propertyListBytes.Length);

                    Bytes bytes = new Bytes();
                    bytes.Append(BitConverter.GetBytes(length));
                    bytes.Append(BitConverter.GetBytes(Properties.Count));
                    bytes.Append(propertyListBytes);
                    return bytes;
                }
            }

            /// <summary>
            /// Gets or sets the Format ID of this MetadataStream Section.
            /// </summary>
            public byte[] FormatId
            {
                get { return _formatId; }
                set
                {
                    if (value == null || value.Length != 16)
                        throw new ArgumentException("Section FormatId must be 16 bytes in length and cannot be null");

                    _formatId = value;
                }
            }

            /// <summary>
            /// Gets the PropertyList of this Section.
            /// </summary>
            public PropertyList Properties
            {
                get { return _properties; }
            }

            /// <summary>
            /// Sets the Property of the specified id and type to the supplied value.
            /// </summary>
            /// <param name="id">The id of the Property whose value is to be set.</param>
            /// <param name="type">The type of the Property whose value is to be set.</param>
            /// <param name="value">The value to which the specified Property is to be set.</param>
            protected void SetProperty(uint id, Property.Types type, object value)
            {
                if (value == null)
                {
                    if (Properties.ContainsKey(id))
                        Properties.Remove(id);
                    return;
                }

                Properties.Add(new Property(id, type, value), true);
            }

            /// <summary>
            /// Gets the value of the Property with the specified id.
            /// </summary>
            /// <param name="id">The id of the Property whose value is to be returned.</param>
            /// <returns>The value of the specified Property.</returns>
            protected object GetProperty(uint id)
            {
                if (!Properties.ContainsKey(id))
                    return null;

                return Properties[id].Value;
            }
        }
    }
}
namespace Core.MyOle2.Metadata
{
    public partial class MetadataStream
    {
        /// <summary>
        /// Manages the list of Sections for a SummaryInformation object.
        /// </summary>
        public class SectionList
        {
            private List<Section> _sections = new List<Section>();

            internal Bytes Bytes
            {
                get
                {
                    Bytes bytes = new Bytes();

                    List<Bytes> sectionBytesList = new List<Bytes>();

                    int offset = 28 + (20 * _sections.Count);
                    foreach (Section section in _sections)
                    {
                        bytes.Append(section.FormatId);
                        bytes.Append(BitConverter.GetBytes((uint)offset));
                        sectionBytesList.Add(section.Bytes);
                        offset += sectionBytesList[sectionBytesList.Count - 1].Length;
                    }

                    foreach (Bytes sectionBytesListItem in sectionBytesList)
                        bytes.Append(sectionBytesListItem);

                    return bytes;
                }
            }

            /// <summary>
            /// Gets the number of Section objects in this SectionList.
            /// </summary>
            public uint Count
            {
                get { return (uint)_sections.Count; }
            }

            /// <summary>
            /// Adds a Section to this MetadataStream's Sections collection.
            /// </summary>
            /// <param name="section"><see crefk="Section" /> to add.</param>
            public void Add(Section section)
            {
                _sections.Add(section);
            }
        }
    }
}
namespace Core.MyOle2.Metadata
{
    /// <summary>
    /// Represents a SummaryInformation stream in an Ole2 Document.
    /// </summary>
    public class SummaryInformationSection : MetadataStream.Section
    {
        private static readonly byte[] FORMAT_ID = new byte[] {
                0xE0, 0x85, 0x9F, 0xF2, 0xF9, 0x4F, 0x68, 0x10, 0xAB, 0x91, 0x08, 0x00, 0x2B, 0x27, 0xB3, 0xD9 };

        private const uint ID_CODEPAGE = 1; //http://msdn2.microsoft.com/en-us/library/aa372045.aspx
        private const uint ID_TITLE = 2;
        private const uint ID_SUBJECT = 3;
        private const uint ID_AUTHOR = 4;
        private const uint ID_KEYWORDS = 5;
        private const uint ID_COMMENTS = 6;
        private const uint ID_TEMPLATE = 7;
        private const uint ID_LAST_SAVED_BY = 8;
        private const uint ID_REVISION_NUMBER = 9;
        private const uint ID_TOTAL_EDITING_TIME = 10;
        private const uint ID_LAST_PRINTED = 11;
        private const uint ID_CREATE_TIME_DATE = 12;
        private const uint ID_LAST_SAVED_TIME_DATE = 13;
        private const uint ID_NUMBER_OF_PAGES = 14;
        private const uint ID_NUMBER_OF_WORDS = 15;
        private const uint ID_NUMBER_OF_CHARACTERS = 16;
        private const uint ID_THUMBNAIL = 17;
        private const uint ID_NAME_OF_CREATING_APPLICATION = 18;
        private const uint ID_SECURITY = 19;

        /// <summary>
        /// Initializes a new instance of the SummaryInformationSection class.
        /// </summary>
        public SummaryInformationSection()
        {
            FormatId = FORMAT_ID;
            CodePage = 1252;
            LastSavedBy = Environment.UserName;
            NameOfCreatingApplication = "Core.MyXls";
            Comments = "This workbook generated by MyXls! http://sourceforge.net/myxls";
            CreateTimeDate = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the CodePage of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public short? CodePage
        {
            get { return (short?)GetProperty(ID_CODEPAGE); }
            set { SetProperty(ID_CODEPAGE, Property.Types.VT_I2, value); }
        }

        /// <summary>
        /// Gets or sets the Title of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Title
        {
            get { return (string)GetProperty(ID_TITLE); }
            set { SetProperty(ID_TITLE, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Subject of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Subject
        {
            get { return (string)GetProperty(ID_SUBJECT); }
            set { SetProperty(ID_SUBJECT, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Author of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Author
        {
            get { return (string)GetProperty(ID_AUTHOR); }
            set { SetProperty(ID_AUTHOR, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Keywords of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Keywords
        {
            get { return (string)GetProperty(ID_KEYWORDS); }
            set { SetProperty(ID_KEYWORDS, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Comments of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Comments
        {
            get { return (string)GetProperty(ID_COMMENTS); }
            set { SetProperty(ID_COMMENTS, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Template of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string Template
        {
            get { return (string)GetProperty(ID_TEMPLATE); }
            set { SetProperty(ID_TEMPLATE, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the LastSavedBy of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string LastSavedBy
        {
            get { return (string)GetProperty(ID_LAST_SAVED_BY); }
            set { SetProperty(ID_LAST_SAVED_BY, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the RevisionNumber of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string RevisionNumber
        {
            get { return (string)GetProperty(ID_REVISION_NUMBER); }
            set { SetProperty(ID_REVISION_NUMBER, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the TotalEditingTime of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public DateTime? TotalEditingTime
        {
            get { return (DateTime?)GetProperty(ID_TOTAL_EDITING_TIME); }
            set { SetProperty(ID_TOTAL_EDITING_TIME, Property.Types.VT_FILETIME, value); }
        }

        /// <summary>
        /// Gets or sets the LastPrinted Date/Time of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public DateTime? LastPrinted
        {
            get { return (DateTime?)GetProperty(ID_LAST_PRINTED); }
            set { SetProperty(ID_LAST_PRINTED, Property.Types.VT_FILETIME, value); }
        }

        /// <summary>
        /// Gets or sets the CreateDateTime of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public DateTime? CreateTimeDate
        {
            get { return (DateTime?)GetProperty(ID_CREATE_TIME_DATE); }
            set { SetProperty(ID_CREATE_TIME_DATE, Property.Types.VT_FILETIME, value); }
        }

        /// <summary>
        /// Gets or sets the LastSavedDateTime of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public DateTime? LastSavedTimeDate
        {
            get { return (DateTime?)GetProperty(ID_LAST_SAVED_TIME_DATE); }
            set { SetProperty(ID_LAST_SAVED_TIME_DATE, Property.Types.VT_FILETIME, value); }
        }

        /// <summary>
        /// Gets or sets the NumberOfPages of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? NumberOfPages
        {
            get { return (int?)GetProperty(ID_NUMBER_OF_PAGES); }
            set { SetProperty(ID_NUMBER_OF_PAGES, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the NumberOfWords of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? NumberOfWords
        {
            get { return (int?)GetProperty(ID_NUMBER_OF_WORDS); }
            set { SetProperty(ID_NUMBER_OF_WORDS, Property.Types.VT_I4, value); }
        }

        /// <summary>
        /// Gets or sets the NumberOfCharacters of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? NumberOfCharacters
        {
            get { return (int?)GetProperty(ID_NUMBER_OF_CHARACTERS); }
            set { SetProperty(ID_NUMBER_OF_CHARACTERS, Property.Types.VT_I4, value); }
        }

        //        public object Thumbnail
        //        {
        //            get { return _thumbnail; }
        //            set { _thumbnail = value; }
        //        }

        /// <summary>
        /// Gets or sets the NameOfCreatingApplication of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public string NameOfCreatingApplication
        {
            get { return (string)GetProperty(ID_NAME_OF_CREATING_APPLICATION); }
            set { SetProperty(ID_NAME_OF_CREATING_APPLICATION, Property.Types.VT_LPSTR, value); }
        }

        /// <summary>
        /// Gets or sets the Security of this SummaryInformation Section.  Setting to null removes the property.
        /// </summary>
        public int? Security
        {
            get { return (int?)GetProperty(ID_SECURITY); }
            set { SetProperty(ID_SECURITY, Property.Types.VT_I4, value); }
        }
    }
}
