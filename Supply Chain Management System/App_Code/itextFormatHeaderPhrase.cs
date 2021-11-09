using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;

public class itextFormatHeaderPhrase
{
    public itextFormatHeaderPhrase()
    {
        //
        // TODO: Add constructor logic here
        //
    } 

    public Phrase FormatHeader15_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader15_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader15_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.ITALIC));
    }

    public Phrase FormatHeader15_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 14-px *******//

    public Phrase FormatHeader14_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader14_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader14_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader14_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 13-px *******//

    public Phrase FormatHeader13_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 13, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader13_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 13, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader13_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 13, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader13_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 13, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 12-px *******//

    public Phrase FormatHeader12_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader12_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader12_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader12_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 11-px *******//

    public Phrase FormatHeader11_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader11_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.NORMAL));
    }

    public Phrase FormatHeader11_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader11_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 10-px *******//

    public Phrase FormatHeader10_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader10_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader10_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader10_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 9-px *******//

    public Phrase FormatHeader9_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader9_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader9_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader9_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 8-px *******//
    public Phrase FormatHeader8_BOLD(int value)
    {
        string value2 = Convert.ToString(value);
    
        return new Phrase(value2, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader8_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader8_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader8_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader8_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 7-px *******//

    public Phrase FormatHeader7_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader7_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader7_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader7_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLDITALIC));
    }

    // ********************* Font Siza 6-px *******//

    public Phrase FormatHeader6_BOLD(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6, iTextSharp.text.Font.BOLD));
    }
    public Phrase FormatHeader6_NORMAL(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6, iTextSharp.text.Font.NORMAL));
    }
    public Phrase FormatHeader6_ITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6, iTextSharp.text.Font.ITALIC));
    }
    public Phrase FormatHeader6_BOLDITALIC(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6, iTextSharp.text.Font.BOLDITALIC));
    }
}
