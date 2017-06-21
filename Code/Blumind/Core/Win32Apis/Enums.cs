using System;

namespace Blumind.Controls.OS
{
    public enum ROP2 : uint
    {
        /* Ternary raster operations */
        SRCCOPY	    = 0x00CC0020, /* dest = source                   */
        SRCPAINT	= 0x00EE0086, /* dest = source OR dest           */
        SRCAND	    = 0x008800C6, /* dest = source AND dest          */
        SRCINVERT	= 0x00660046, /* dest = source XOR dest          */
        SRCERASE	= 0x00440328, /* dest = source AND (NOT dest )   */
        NOTSRCCOPY	= 0x00330008, /* dest = (NOT source)             */
        NOTSRCERASE	= 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
        MERGECOPY	= 0x00C000CA, /* dest = (source AND pattern)     */
        MERGEPAINT	= 0x00BB0226, /* dest = (NOT source) OR dest     */
        PATCOPY	    = 0x00F00021, /* dest = pattern                  */
        PATPAINT	= 0x00FB0A09, /* dest = DPSnoo                   */
        PATINVERT	= 0x005A0049, /* dest = pattern XOR dest         */
        DSTINVERT	= 0x00550009, /* dest = (NOT dest)               */
        BLACKNESS	= 0x00000042, /* dest = BLACK                    */
        WHITENESS	= 0x00FF0062, /* dest = WHITE                    */
        NOMIRRORBITMAP	= 0x80000000, /* Do not Mirror the bitmap in this call */
        CAPTUREBLT	= 0x40000000, /* Include layered windows */
    }

    public static class WindowHandle
    {
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    }

    public enum ShowWindowFlags
    {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_SHOWDEFAULT = 10,
        SW_FORCEMINIMIZE = 11,
        SW_MAX = 11,
    }

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOREDRAW = 0x0008,
        SWP_NOACTIVATE = 0x0010,
        SWP_FRAMECHANGED = 0x0020,  /* The frame changed: send WM_NCCALCSIZE */
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_NOCOPYBITS = 0x0100,
        SWP_NOOWNERZORDER = 0x0200,  /* Don't do owner Z ordering */
        SWP_NOSENDCHANGING = 0x0400,  /* Don't send WM_WINDOWPOSCHANGING */
        SWP_DRAWFRAME = SWP_FRAMECHANGED,
        SWP_NOREPOSITION = SWP_NOOWNERZORDER,
        SWP_DEFERERASE = 0x2000,
        SWP_ASYNCWINDOWPOS = 0x4000,
    }

    public enum RopModes
    {
        /* Binary raster ops */
        R2_BLACK = 1,       /*  0       */
        R2_NOTMERGEPEN = 2, /* DPon     */
        R2_MASKNOTPEN = 3,  /* DPna     */
        R2_NOTCOPYPEN = 4,  /* PN       */
        R2_MASKPENNOT = 5,  /* PDna     */
        R2_NOT = 6,         /* Dn       */
        R2_XORPEN = 7,      /* DPx      */
        R2_NOTMASKPEN = 8,  /* DPan     */
        R2_MASKPEN = 9,     /* DPa      */
        R2_NOTXORPEN = 10,  /* DPxn     */
        R2_NOP = 11,        /* D        */
        R2_MERGENOTPEN = 12,    /* DPno     */
        R2_COPYPEN = 13,    /* P        */
        R2_MERGEPENNOT = 14,    /* PDno     */
        R2_MERGEPEN = 15,   /* DPo      */
        R2_WHITE = 16,  /*  1       */
        R2_LAST = 16,
    }

    public enum StockObjects
    {
        GRAY_BRUSH = 2,
        DKGRAY_BRUSH = 3,
        BLACK_BRUSH = 4,
        NULL_BRUSH = 5,
        HOLLOW_BRUSH = NULL_BRUSH,
        WHITE_PEN = 6,
        BLACK_PEN = 7,
        NULL_PEN = 8,
        OEM_FIXED_FONT = 10,
        ANSI_FIXED_FONT = 11,
        ANSI_VAR_FONT = 12,
        SYSTEM_FONT = 13,
        DEVICE_DEFAULT_FONT = 14,
        DEFAULT_PALETTE = 15,
        SYSTEM_FIXED_FONT = 16,
        DEFAULT_GUI_FONT = 17,
        DC_BRUSH = 18,
        DC_PEN = 19,
    }

    /* Graphics Modes */
    public enum GraphicsModes
    {
        GM_COMPATIBLE       = 1,
        GM_ADVANCED         = 2,
        GM_LAST             = 2,
    }

    public enum SystemObjects : long
    {
        OBJID_WINDOW = 0x00000000L,
        OBJID_SYSMENU = 0xFFFFFFFFL,
        OBJID_TITLEBAR = 0xFFFFFFFEL,
        OBJID_MENU = 0xFFFFFFFDL,
        OBJID_CLIENT = 0xFFFFFFFCL,
        OBJID_VSCROLL = 0xFFFFFFFBL,
        OBJID_HSCROLL = 0xFFFFFFFAL,
        OBJID_SIZEGRIP = 0xFFFFFFF9L,
        OBJID_CARET = 0xFFFFFFF8L,
        OBJID_CURSOR = 0xFFFFFFF7L,
        OBJID_ALERT = 0xFFFFFFF6L,
        OBJID_SOUND = 0xFFFFFFF5L,
        OBJID_QUERYCLASSNAMEIDX = 0xFFFFFFF4L,
        OBJID_NATIVEOM = 0xFFFFFFF0L,
    }

    public enum Scrollbars
    {
        SB_HORZ = 0,
        SB_VERT = 1,
        SB_CTL  = 2,
        SB_BOTH = 3,
    }

    public enum SetScrollInfoMask : int
    {
        SIF_RANGE = 0x0001,
        SIF_PAGE = 0x0002,
        SIF_POS = 0x0004,
        SIF_DISABLENOSCROLL = 0x0008,
        SIF_TRACKPOS = 0x0010,
        SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
    }

    public enum SystemMetrics
    {
        SM_CXSCREEN             = 0,
	    SM_CYSCREEN             = 1,
	    SM_CXVSCROLL            = 2,
	    SM_CYHSCROLL            = 3,
	    SM_CYCAPTION            = 4,
	    SM_CXBORDER             = 5,
	    SM_CYBORDER             = 6,
	    SM_CXDLGFRAME           = 7,
	    SM_CYDLGFRAME           = 8,
	    SM_CYVTHUMB             = 9,
	    SM_CXHTHUMB             = 10,
	    SM_CXICON               = 11,
	    SM_CYICON               = 12,
	    SM_CXCURSOR             = 13,
	    SM_CYCURSOR             = 14,
	    SM_CYMENU               = 15,
	    SM_CXFULLSCREEN         = 16,
	    SM_CYFULLSCREEN         = 17,
	    SM_CYKANJIWINDOW        = 18,
	    SM_MOUSEPRESENT         = 19,
	    SM_CYVSCROLL            = 20,
	    SM_CXHSCROLL            = 21,
	    SM_DEBUG                = 22,
	    SM_SWAPBUTTON           = 23,
	    SM_RESERVED1            = 24,
	    SM_RESERVED2            = 25,
	    SM_RESERVED3            = 26,
	    SM_RESERVED4            = 27,
	    SM_CXMIN                = 28,
	    SM_CYMIN                = 29,
	    SM_CXSIZE               = 30,
	    SM_CYSIZE               = 31,
	    SM_CXFRAME              = 32,
	    SM_CYFRAME              = 33,
	    SM_CXMINTRACK           = 34,
	    SM_CYMINTRACK           = 35,
	    SM_CXDOUBLECLK          = 36,
	    SM_CYDOUBLECLK          = 37,
	    SM_CXICONSPACING        = 38,
	    SM_CYICONSPACING        = 39,
	    SM_MENUDROPALIGNMENT    = 40,
	    SM_PENWINDOWS           = 41,
	    SM_DBCSENABLED          = 42,
	    SM_CMOUSEBUTTONS        = 43,
    //#if(WINVER >= 0x0400)
	    SM_CXFIXEDFRAME         = SM_CXDLGFRAME,  /* ;win40 name change */
	    SM_CYFIXEDFRAME         = SM_CYDLGFRAME,  /* ;win40 name change */
	    SM_CXSIZEFRAME          = SM_CXFRAME,     /* ;win40 name change */
	    SM_CYSIZEFRAME          = SM_CYFRAME,     /* ;win40 name change */
	    SM_SECURE               = 44,
	    SM_CXEDGE               = 45,
	    SM_CYEDGE               = 46,
	    SM_CXMINSPACING         = 47,
	    SM_CYMINSPACING         = 48,
	    SM_CXSMICON             = 49,
	    SM_CYSMICON             = 50,
	    SM_CYSMCAPTION          = 51,
	    SM_CXSMSIZE             = 52,
	    SM_CYSMSIZE             = 53,
	    SM_CXMENUSIZE           = 54,
	    SM_CYMENUSIZE           = 55,
	    SM_ARRANGE              = 56,
	    SM_CXMINIMIZED          = 57,
	    SM_CYMINIMIZED          = 58,
	    SM_CXMAXTRACK           = 59,
	    SM_CYMAXTRACK           = 60,
	    SM_CXMAXIMIZED          = 61,
	    SM_CYMAXIMIZED          = 62,
	    SM_NETWORK              = 63,
	    SM_CLEANBOOT            = 67,
	    SM_CXDRAG               = 68,
	    SM_CYDRAG               = 69,
    //#endif /* WINVER >= 0x0400 */
	    SM_SHOWSOUNDS           = 70,
    //#if(WINVER >= 0x0400)= ,
	    SM_CXMENUCHECK          = 71,   /* Use instead of GetMenuCheckMarkDimensions()! */
	    SM_CYMENUCHECK          = 72,
	    SM_SLOWMACHINE          = 73,
	    SM_MIDEASTENABLED       = 74,
    //#endif /* WINVER >= 0x0400 */

    //#if (WINVER >= 0x0500) || (_WIN32_WINNT >= 0x0400)
	    SM_MOUSEWHEELPRESENT    = 75,
    //#endif
    //#if(WINVER >= 0x0500)
	    SM_XVIRTUALSCREEN       = 76,
	    SM_YVIRTUALSCREEN       = 77,
	    SM_CXVIRTUALSCREEN      = 78,
	    SM_CYVIRTUALSCREEN      = 79,
	    SM_CMONITORS            = 80,
	    SM_SAMEDISPLAYFORMAT    = 81,
    //#endif /* WINVER >= 0x0500 */
    //#if(_WIN32_WINNT >= 0x0500)
	    SM_IMMENABLED           = 82,
    //#endif /* _WIN32_WINNT >= 0x0500 */
    //#if(_WIN32_WINNT >= 0x0501)= ,
	    SM_CXFOCUSBORDER        = 83,
	    SM_CYFOCUSBORDER        = 84,
    //#endif /* _WIN32_WINNT >= 0x0501 */

    //#if(_WIN32_WINNT >= 0x0501)
	    SM_TABLETPC             = 86,
	    SM_MEDIACENTER          = 87,
	    SM_STARTER              = 88,
	    SM_SERVERR2             = 89,
    //#endif /* _WIN32_WINNT >= 0x0501 */
    }

    public enum PenStyles
    {
        /* Pen Styles */
        PS_SOLID = 0,
        PS_DASH = 1,       /* -------  */
        PS_DOT = 2,       /* .......  */
        PS_DASHDOT = 3,       /* _._._._  */
        PS_DASHDOTDOT = 4,       /* _.._.._  */
        PS_NULL = 5,
        PS_INSIDEFRAME = 6,
        PS_USERSTYLE = 7,
        PS_ALTERNATE = 8,

        PS_STYLE_MASK = 0x0000000F,
        PS_ENDCAP_ROUND = 0x00000000,
        PS_ENDCAP_SQUARE = 0x00000100,
        PS_ENDCAP_FLAT = 0x00000200,
        PS_ENDCAP_MASK = 0x00000F00,
        PS_JOIN_ROUND = 0x00000000,
        PS_JOIN_BEVEL = 0x00001000,
        PS_JOIN_MITER = 0x00002000,
        PS_JOIN_MASK = 0x0000F000,
        PS_COSMETIC = 0x00000000,
        PS_GEOMETRIC = 0x00010000,
        PS_TYPE_MASK = 0x000F0000,
    }

    public enum GetWindowLongFlags
    {
        GWL_WNDPROC = (-4),
        GWL_HINSTANCE = (-6),
        GWL_HWNDPARENT = (-8),
        GWL_STYLE = (-16),
        GWL_EXSTYLE = (-20),
        GWL_USERDATA = (-21),
        GWL_ID = (-12),
        DWL_MSGRESULT = 0,
        DWL_DLGPROC = 4,
        DWL_USER = 8,
    }

    public enum GetClassLongFlags
    {
        GCL_MENUNAME = (-8),
        GCL_HBRBACKGROUND = (-10),
        GCL_HCURSOR = (-12),
        GCL_HICON = (-14),
        GCL_HMODULE = (-16),
        GCL_CBWNDEXTRA = (-18),
        GCL_CBCLSEXTRA = (-20),
        GCL_WNDPROC = (-24),
        GCL_STYLE = (-26),
        GCW_ATOM = (-32),
    }

    [Flags]
    public enum ClassStyles
    {
        CS_VREDRAW = 0x0001,
        CS_HREDRAW = 0x0002,
        CS_DBLCLKS = 0x0008,
        CS_OWNDC = 0x0020,
        CS_CLASSDC = 0x0040,
        CS_PARENTDC = 0x0080,
        CS_NOCLOSE = 0x0200,
        CS_SAVEBITS = 0x0800,
        CS_BYTEALIGNCLIENT = 0x1000,
        CS_BYTEALIGNWINDOW = 0x2000,
        CS_GLOBALCLASS = 0x4000,
        CS_IME = 0x00010000,
        CS_DROPSHADOW = 0x00020000,
    }

    public enum THEMESIZE 
    {
        TS_MIN,
        TS_TRUE,
        TS_DRAW
    }

    /* Device Parameters for GetDeviceCaps() */
    public enum DeviceCapsIndex
    {
        DRIVERVERSION = 0,     /* Device driver version                    */
        TECHNOLOGY = 2,     /* Device classification                    */
        HORZSIZE = 4,     /* Horizontal size in millimeters           */
        VERTSIZE = 6,     /* Vertical size in millimeters             */
        HORZRES = 8,     /* Horizontal width in pixels               */
        VERTRES = 10,    /* Vertical height in pixels                */
        BITSPIXEL = 12,    /* Number of bits per pixel                 */
        PLANES = 14,    /* Number of planes                         */
        NUMBRUSHES = 16,    /* Number of brushes the device has         */
        NUMPENS = 18,    /* Number of pens the device has            */
        NUMMARKERS = 20,    /* Number of markers the device has         */
        NUMFONTS = 22,    /* Number of fonts the device has           */
        NUMCOLORS = 24,    /* Number of colors the device supports     */
        PDEVICESIZE = 26,    /* Size required for device descriptor      */
        CURVECAPS = 28,    /* Curve capabilities                       */
        LINECAPS = 30,    /* Line capabilities                        */
        POLYGONALCAPS = 32,    /* Polygonal capabilities                   */
        TEXTCAPS = 34,    /* Text capabilities                        */
        CLIPCAPS = 36,    /* Clipping capabilities                    */
        RASTERCAPS = 38,    /* Bitblt capabilities                      */
        ASPECTX = 40,    /* Length of the X leg                      */
        ASPECTY = 42,    /* Length of the Y leg                      */
        ASPECTXY = 44,    /* Length of the hypotenuse                 */

        LOGPIXELSX = 88,    /* Logical pixels/inch in X                 */
        LOGPIXELSY = 90,    /* Logical pixels/inch in Y                 */

        SIZEPALETTE = 104,    /* Number of entries in physical palette    */
        NUMRESERVED = 106,    /* Number of reserved entries in palette    */
        COLORRES = 108,    /* Actual color resolution                  */

        // Printing related DeviceCaps. These replace the appropriate Escapes

        PHYSICALWIDTH = 110, /* Physical Width in device units           */
        PHYSICALHEIGHT = 111, /* Physical Height in device units          */
        PHYSICALOFFSETX = 112, /* Physical Printable Area x margin         */
        PHYSICALOFFSETY = 113, /* Physical Printable Area y margin         */
        SCALINGFACTORX = 114, /* Scaling factor x                         */
        SCALINGFACTORY = 115, /* Scaling factor y                         */

        // Display driver specific

        VREFRESH = 116,  /* Current vertical refresh rate of the    */
        /* display device (for displays only) in Hz*/
        DESKTOPVERTRES = 117,  /* Horizontal width of entire desktop in   */
        /* pixels                                  */
        DESKTOPHORZRES = 118,  /* Vertical height of entire desktop in    */
        /* pixels                                  */
        BLTALIGNMENT = 119,  /* Preferred blt alignment                 */

        SHADEBLENDCAPS = 120,  /* Shading and blending caps               */
        COLORMGMTCAPS = 121,  /* Color Management caps                   */

    }

    [Flags]
    public enum WindowStyle : uint
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = 0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,     /* WS_BORDER | WS_DLGFRAME  */
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,

        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,


        WS_TILED = WS_OVERLAPPED,
        WS_ICONIC = WS_MINIMIZE,
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

        /*
         * Common Window Styles
         */
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED |
                             WS_CAPTION |
                             WS_SYSMENU |
                             WS_THICKFRAME |
                             WS_MINIMIZEBOX |
                             WS_MAXIMIZEBOX),

        WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),

        WS_CHILDWINDOW = (WS_CHILD),
    }
}
