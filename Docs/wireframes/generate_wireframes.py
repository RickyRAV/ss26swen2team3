"""
Generate TourPlanner wireframe PNGs using Pillow.
Run: pip3 install Pillow && python3 generate_wireframes.py
"""

from PIL import Image, ImageDraw, ImageFont
import os

OUT = os.path.dirname(__file__)
W, H = 1280, 800
BG = "#F5F5F4"          # stone-100
WHITE = "#FFFFFF"
BORDER = "#D6D3D1"      # stone-300
TEXT_DARK = "#1C1917"   # stone-900
TEXT_MID = "#78716C"    # stone-500
TEXT_LIGHT = "#A8A29E"  # stone-400
PRIMARY = "#166534"     # forest green
PRIMARY_LIGHT = "#DCFCE7"
ACCENT = "#92400E"      # trail amber

def font(size=14):
    try:
        return ImageFont.truetype("/System/Library/Fonts/Helvetica.ttc", size)
    except Exception:
        return ImageFont.load_default()

def rect(draw, x, y, w, h, fill=WHITE, outline=BORDER, radius=6):
    draw.rounded_rectangle([x, y, x+w, y+h], radius=radius, fill=fill, outline=outline)

def label(draw, x, y, text, size=13, color=TEXT_DARK, bold=False):
    f = font(size)
    draw.text((x, y), text, fill=color, font=f)

def button(draw, x, y, w, h, text, primary=False):
    fill = PRIMARY if primary else WHITE
    fg = WHITE if primary else TEXT_DARK
    rect(draw, x, y, w, h, fill=fill, outline=PRIMARY if primary else BORDER)
    tw, th = draw.textlength(text, font=font(13)), 14
    draw.text((x + w//2 - int(tw)//2, y + h//2 - 8), text, fill=fg, font=font(13))

def input_field(draw, x, y, w, placeholder="", value=""):
    rect(draw, x, y, w, 36, fill=WHITE, outline=BORDER)
    draw.text((x+10, y+10), value if value else placeholder,
              fill=TEXT_DARK if value else TEXT_LIGHT, font=font(13))

def navbar(draw, title="TourPlanner"):
    draw.rectangle([0, 0, W, 48], fill=PRIMARY)
    draw.text((20, 14), title, fill=WHITE, font=font(16))
    draw.text((W-80, 14), "Log out", fill=WHITE, font=font(13))

# ─── 1. Login Screen ─────────────────────────────────────────────────────────
def wireframe_login():
    img = Image.new("RGB", (W, H), BG)
    d = ImageDraw.Draw(img)

    # Center card
    cx, cy, cw, ch = W//2-200, H//2-220, 400, 380
    rect(d, cx, cy, cw, ch, fill=WHITE, outline=BORDER, radius=12)

    label(d, cx+20, cy+24, "TourPlanner", size=20, color=TEXT_DARK)
    label(d, cx+20, cy+52, "Sign in to your account", size=13, color=TEXT_MID)

    label(d, cx+20, cy+90, "Email address", size=12, color=TEXT_DARK)
    input_field(d, cx+20, cy+108, 360, placeholder="you@example.com")

    label(d, cx+20, cy+158, "Password", size=12, color=TEXT_DARK)
    input_field(d, cx+20, cy+176, 360, placeholder="••••••••")

    button(d, cx+20, cy+228, 360, 40, "Sign in", primary=True)

    d.line([cx+20, cy+290, cx+172, cy+290], fill=BORDER)
    label(d, cx+180, cy+282, "or", size=12, color=TEXT_MID)
    d.line([cx+208, cy+290, cx+380, cy+290], fill=BORDER)

    label(d, cx+60, cy+310, "Don't have an account?  ", size=13, color=TEXT_MID)
    label(d, cx+228, cy+310, "Register", size=13, color=PRIMARY)

    label(d, cx+90, cy+348, "← Back to tour list", size=12, color=TEXT_LIGHT)

    label(d, 20, H-24, "01 — Login", size=11, color=TEXT_LIGHT)
    img.save(os.path.join(OUT, "01_login.png"))
    print("Saved 01_login.png")

# ─── 2. Register Screen ───────────────────────────────────────────────────────
def wireframe_register():
    img = Image.new("RGB", (W, H), BG)
    d = ImageDraw.Draw(img)

    cx, cy, cw, ch = W//2-200, H//2-240, 400, 440
    rect(d, cx, cy, cw, ch, fill=WHITE, outline=BORDER, radius=12)

    label(d, cx+20, cy+24, "Create an account", size=18, color=TEXT_DARK)
    label(d, cx+20, cy+52, "Start planning your tours", size=13, color=TEXT_MID)

    label(d, cx+20, cy+90, "Username", size=12, color=TEXT_DARK)
    input_field(d, cx+20, cy+108, 360, placeholder="johndoe")

    label(d, cx+20, cy+158, "Email address", size=12, color=TEXT_DARK)
    input_field(d, cx+20, cy+176, 360, placeholder="you@example.com")

    label(d, cx+20, cy+226, "Password", size=12, color=TEXT_DARK)
    input_field(d, cx+20, cy+244, 360, placeholder="••••••••")

    label(d, cx+20, cy+294, "Confirm password", size=12, color=TEXT_DARK)
    input_field(d, cx+20, cy+312, 360, placeholder="••••••••")

    button(d, cx+20, cy+362, 360, 40, "Create account", primary=True)

    label(d, cx+70, cy+418, "Already have an account?  ", size=13, color=TEXT_MID)
    label(d, cx+248, cy+418, "Sign in", size=13, color=PRIMARY)

    label(d, 20, H-24, "02 — Register", size=11, color=TEXT_LIGHT)
    img.save(os.path.join(OUT, "02_register.png"))
    print("Saved 02_register.png")

# ─── 3. Tour List + Detail (main layout) ─────────────────────────────────────
def wireframe_main():
    img = Image.new("RGB", (W, H), BG)
    d = ImageDraw.Draw(img)
    navbar(d)

    # Left sidebar (tour list)
    SBW = 320
    rect(d, 0, 48, SBW, H-48, fill=WHITE, outline=BORDER, radius=0)

    # Sidebar header
    label(d, 16, 64, "My Tours", size=15, color=TEXT_DARK)
    button(d, SBW-80, 60, 72, 30, "+ New Tour", primary=True)

    # Search bar
    rect(d, 12, 100, SBW-24, 34, fill="#F9F8F7", outline=BORDER)
    label(d, 22, 110, "🔍  Search tours…", size=12, color=TEXT_LIGHT)

    # Tour list items
    tours = [
        ("🥾", "Vienna Forest Hike", "Vienna → Baden", "12.4 km  •  2h 40m  •  3 logs"),
        ("🚴", "Danube Cycling Route", "Vienna → Tulln", "45.0 km  •  3h 10m  •  1 log"),
        ("🏃", "Prater Morning Run", "Prater → Lusthaus", "8.2 km  •  52m  •  5 logs"),
        ("🚗", "Wachau Valley Drive", "Krems → Melk", "38.0 km  •  1h 05m  •  0 logs"),
    ]
    for i, (icon, name, route, meta) in enumerate(tours):
        y = 144 + i * 72
        bg = PRIMARY_LIGHT if i == 0 else WHITE
        bord = PRIMARY if i == 0 else BORDER
        d.rectangle([0, y, SBW, y+70], fill=bg)
        if i == 0:
            d.rectangle([0, y, 3, y+70], fill=PRIMARY)
        label(d, 14, y+10, icon, size=20)
        label(d, 50, y+10, name, size=13, color=TEXT_DARK)
        label(d, 50, y+28, route, size=11, color=TEXT_MID)
        label(d, 50, y+44, meta, size=11, color=TEXT_LIGHT)
        d.line([0, y+70, SBW, y+70], fill=BORDER)

        # Edit/Delete icons
        label(d, SBW-50, y+16, "✎", size=13, color=TEXT_LIGHT)
        label(d, SBW-26, y+16, "🗑", size=13, color=TEXT_LIGHT)

    # Right: Tour detail
    DX = SBW + 1
    DW = W - DX

    # Detail header
    rect(d, DX, 48, DW, 170, fill=WHITE, outline=BORDER, radius=0)
    label(d, DX+20, 62, "🥾  Vienna Forest Hike", size=18, color=TEXT_DARK)
    label(d, DX+20, 92, "📍  Vienna → Baden", size=13, color=TEXT_MID)
    label(d, DX+20, 112, "A scenic hike through the Vienna Woods with gentle elevation changes.", size=12, color=TEXT_MID)

    # Stat cards
    cards = [("📏 Distance", "12.4 km"), ("⏱ Est. Time", "2h 40m"), ("⭐ Popularity", "3 logs"), ("👶 Child-Friendly", "Suitable")]
    for i, (lbl, val) in enumerate(cards):
        cx2 = DX + 20 + i * ((DW-40)//4 + 4)
        rect(d, cx2, 138, (DW-56)//4, 54, fill="#F9F8F7", outline=BORDER)
        label(d, cx2+8, 148, lbl, size=10, color=TEXT_LIGHT)
        label(d, cx2+8, 164, val, size=13, color=TEXT_DARK)

    # Map placeholder
    rect(d, DX+20, 228, DW-40, 180, fill="#EFF6FF", outline=BORDER, radius=8)
    label(d, DX + DW//2 - 60, 298, "📍  Route Map", size=14, color=TEXT_MID)
    label(d, DX + DW//2 - 100, 322, "OpenRouteService / Leaflet goes here", size=11, color=TEXT_LIGHT)

    # Tour image
    rect(d, DX+20, 420, DW-40, 100, fill="#F5F5F4", outline=BORDER, radius=8)
    label(d, DX + DW//2 - 60, 462, "🖼  Tour Image", size=14, color=TEXT_MID)
    label(d, DX + DW//2 - 70, 484, "Uploaded image appears here", size=11, color=TEXT_LIGHT)

    # Tour logs header
    label(d, DX+20, 534, "Tour Logs", size=14, color=TEXT_DARK)
    label(d, DX+44, 536, "(3)", size=11, color=TEXT_LIGHT)
    button(d, W-120, 528, 100, 30, "+ Add Log")

    # Log table header
    d.rectangle([DX+20, 572, W-20, 594], fill="#F9F8F7")
    for col, tx in [("Date", DX+30), ("Comment", DX+130), ("Difficulty", DX+310), ("Distance", DX+420), ("Rating", DX+520)]:
        label(d, tx, 578, col, size=10, color=TEXT_MID)

    # Log rows
    logs = [
        ("2026-04-01", "Great trail conditions", "Easy", "11.8 km", "★★★★☆"),
        ("2026-03-15", "A bit muddy near the end", "Medium", "13.1 km", "★★★☆☆"),
        ("2026-02-28", "Perfect spring weather", "Easy", "12.0 km", "★★★★★"),
    ]
    for i, (date, comment, diff, dist, rating) in enumerate(logs):
        y = 600 + i * 36
        d.rectangle([DX+20, y, W-20, y+34], fill=WHITE if i%2==0 else "#FAFAF9")
        label(d, DX+30, y+10, date, size=11, color=TEXT_MID)
        label(d, DX+130, y+10, comment, size=11, color=TEXT_DARK)
        label(d, DX+310, y+10, diff, size=11, color=TEXT_DARK)
        label(d, DX+420, y+10, dist, size=11, color=TEXT_MID)
        label(d, DX+520, y+10, rating, size=11, color=ACCENT)
        label(d, W-64, y+8, "✎  🗑", size=11, color=TEXT_LIGHT)
        d.line([DX+20, y+34, W-20, y+34], fill=BORDER)

    label(d, 20, H-24, "03 — Main View (Tour List + Tour Detail)", size=11, color=TEXT_LIGHT)
    img.save(os.path.join(OUT, "03_main_view.png"))
    print("Saved 03_main_view.png")

# ─── 4. Tour Form Dialog ──────────────────────────────────────────────────────
def wireframe_tour_form():
    img = Image.new("RGB", (W, H), BG)
    d = ImageDraw.Draw(img)
    navbar(d)

    # Dim background
    d.rectangle([0, 48, W, H], fill="#00000022")

    # Dialog
    dx, dy, dw, dh = W//2-220, H//2-260, 440, 560
    rect(d, dx, dy, dw, dh, fill=WHITE, outline=BORDER, radius=10)

    label(d, dx+20, dy+20, "Create New Tour", size=17, color=TEXT_DARK)
    label(d, dx+dw-20, dy+20, "✕", size=15, color=TEXT_MID)

    label(d, dx+20, dy+58, "Tour name *", size=12)
    input_field(d, dx+20, dy+76, dw-40, placeholder="Morning Bike Ride")

    label(d, dx+20, dy+126, "From *", size=12)
    input_field(d, dx+20, dy+144, (dw-48)//2, placeholder="Vienna")

    label(d, dx+20+(dw-48)//2+8, dy+126, "To *", size=12)
    input_field(d, dx+20+(dw-48)//2+8, dy+144, (dw-48)//2, placeholder="Graz")

    label(d, dx+20, dy+194, "Transport type", size=12)
    rect(d, dx+20, dy+212, dw-40, 36, fill=WHITE, outline=BORDER)
    label(d, dx+30, dy+222, "🥾  Hiking", size=13, color=TEXT_DARK)
    label(d, dx+dw-52, dy+222, "▼", size=11, color=TEXT_MID)

    label(d, dx+20, dy+262, "Description", size=12)
    rect(d, dx+20, dy+280, dw-40, 72, fill=WHITE, outline=BORDER, radius=4)
    label(d, dx+30, dy+294, "A scenic route through the hills…", size=12, color=TEXT_LIGHT)

    label(d, dx+20, dy+366, "Tour Image", size=12)
    rect(d, dx+20, dy+384, dw-40, 56, fill="#F9F8F7", outline="#D6D3D1", radius=4)
    d.line([dx+20, dy+384, dx+20+dw-40, dy+384+56], fill=BORDER, width=1)  # dashed simulation
    label(d, dx+80, dy+406, "🖼  Upload image  (jpg, png, webp — max 10 MB)", size=12, color=TEXT_LIGHT)

    # Error state example
    rect(d, dx+20, dy+452, dw-40, 32, fill="#FEF2F2", outline="#FECACA", radius=4)
    label(d, dx+32, dy+462, "⚠  Name is required", size=12, color="#DC2626")

    button(d, dx+20, dy+500, (dw-48)//2, 40, "Cancel")
    button(d, dx+20+(dw-48)//2+8, dy+500, (dw-48)//2, 40, "Create tour", primary=True)

    label(d, 20, H-24, "04 — Tour Form Dialog (Create / Edit)", size=11, color=TEXT_LIGHT)
    img.save(os.path.join(OUT, "04_tour_form.png"))
    print("Saved 04_tour_form.png")

# ─── 5. Tour Log Form Dialog ──────────────────────────────────────────────────
def wireframe_log_form():
    img = Image.new("RGB", (W, H), BG)
    d = ImageDraw.Draw(img)
    navbar(d)

    d.rectangle([0, 48, W, H], fill="#00000022")

    dx, dy, dw, dh = W//2-220, H//2-240, 440, 480
    rect(d, dx, dy, dw, dh, fill=WHITE, outline=BORDER, radius=10)

    label(d, dx+20, dy+20, "Add Tour Log", size=17, color=TEXT_DARK)
    label(d, dx+dw-20, dy+20, "✕", size=15, color=TEXT_MID)

    label(d, dx+20, dy+58, "Date & Time *", size=12)
    input_field(d, dx+20, dy+76, (dw-48)//2, value="2026-04-11  14:30")

    label(d, dx+20+(dw-48)//2+8, dy+58, "Difficulty", size=12)
    rect(d, dx+20+(dw-48)//2+8, dy+76, (dw-48)//2, 36, fill=WHITE, outline=BORDER)
    label(d, dx+32+(dw-48)//2, dy+86, "Medium  ▼", size=13, color=TEXT_DARK)

    label(d, dx+20, dy+126, "Distance (km)", size=12)
    input_field(d, dx+20, dy+144, (dw-48)//2, value="12.4")

    label(d, dx+20+(dw-48)//2+8, dy+126, "Duration (minutes)", size=12)
    input_field(d, dx+20+(dw-48)//2+8, dy+144, (dw-48)//2, value="160")

    label(d, dx+20, dy+194, "Rating", size=12)
    ratings = ["★", "★★", "★★★", "★★★★", "★★★★★"]
    rw = (dw-40) // 5 - 4
    for i, r in enumerate(ratings):
        rx = dx+20 + i*(rw+4)
        fill = ACCENT if i == 3 else WHITE
        fg = WHITE if i == 3 else TEXT_MID
        rect(d, rx, dy+212, rw, 34, fill=fill, outline=ACCENT if i==3 else BORDER)
        label(d, rx+rw//2-len(r)*5, dy+222, r, size=11, color=fg)

    label(d, dx+20, dy+260, "Comment", size=12)
    rect(d, dx+20, dy+278, dw-40, 72, fill=WHITE, outline=BORDER, radius=4)
    label(d, dx+30, dy+292, "How did it go?", size=12, color=TEXT_LIGHT)

    button(d, dx+20, dy+420, (dw-48)//2, 40, "Cancel")
    button(d, dx+20+(dw-48)//2+8, dy+420, (dw-48)//2, 40, "Add log", primary=True)

    label(d, 20, H-24, "05 — Tour Log Form Dialog (Add / Edit)", size=11, color=TEXT_LIGHT)
    img.save(os.path.join(OUT, "05_log_form.png"))
    print("Saved 05_log_form.png")

# ─── 6. Mobile/Responsive (narrow) ───────────────────────────────────────────
def wireframe_mobile():
    MW, MH = 390, 844
    img = Image.new("RGB", (MW, MH), BG)
    d = ImageDraw.Draw(img)

    # Navbar
    d.rectangle([0, 0, MW, 44], fill=PRIMARY)
    label(d, 14, 12, "TourPlanner", size=14, color=WHITE)

    # Full-width tour list (no detail panel on mobile)
    rect(d, 0, 44, MW, MH-44, fill=WHITE, outline=BORDER, radius=0)
    label(d, 14, 56, "My Tours", size=14, color=TEXT_DARK)
    button(d, MW-82, 50, 76, 30, "+ New Tour", primary=True)

    rect(d, 10, 90, MW-20, 32, fill="#F9F8F7", outline=BORDER)
    label(d, 20, 100, "🔍  Search tours…", size=12, color=TEXT_LIGHT)

    tours = [
        ("🥾", "Vienna Forest Hike", "Vienna → Baden"),
        ("🚴", "Danube Cycling Route", "Vienna → Tulln"),
        ("🏃", "Prater Morning Run", "Prater → Lusthaus"),
        ("🚗", "Wachau Valley Drive", "Krems → Melk"),
    ]
    for i, (icon, name, route) in enumerate(tours):
        y = 132 + i*68
        d.rectangle([0, y, MW, y+66], fill=PRIMARY_LIGHT if i==0 else WHITE)
        if i==0: d.rectangle([0, y, 3, y+66], fill=PRIMARY)
        label(d, 12, y+10, icon, size=18)
        label(d, 44, y+10, name, size=13, color=TEXT_DARK)
        label(d, 44, y+30, route, size=11, color=TEXT_MID)
        label(d, 44, y+46, "Tap to view details →", size=10, color=TEXT_LIGHT)
        d.line([0, y+66, MW, y+66], fill=BORDER)

    label(d, 14, MH-20, "06 — Mobile / Responsive", size=10, color=TEXT_LIGHT)
    img.save(os.path.join(OUT, "06_mobile.png"))
    print("Saved 06_mobile.png")


if __name__ == "__main__":
    wireframe_login()
    wireframe_register()
    wireframe_main()
    wireframe_tour_form()
    wireframe_log_form()
    wireframe_mobile()
    print("\nAll wireframes generated in", OUT)
