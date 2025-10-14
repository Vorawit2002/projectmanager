/**
 * Convert Hex color to rgb
 * @param hex
 */

export const hexToRgb = (hex: string) => {
// Expand shorthand form (e.g. "03F") to full form (e.g. "0033FF")
  const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i

  hex = hex.replace(shorthandRegex, (m: string, r: string, g: string, b: string) => {
    return r + r + g + g + b + b
  })

  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)

  return result ? `${Number.parseInt(result[1], 16)},${Number.parseInt(result[2], 16)},${Number.parseInt(result[3], 16)}` : null
}

/**
 *RGBA color to Hex color with / without opacity
 */
export const rgbaToHex = (rgba: string, forceRemoveAlpha = false) => {
  return (
    `#${
      rgba
        .replace(/^rgba?\(|\s+|\)$/g, '') // Get's rgba / rgb string values
        .split(',') // splits them at ","
        .filter((string, index) => !forceRemoveAlpha || index !== 3)
        .map(string => Number.parseFloat(string)) // Converts them to numbers
        .map((number, index) => (index === 3 ? Math.round(number * 255) : number)) // Converts alpha to 255 number
        .map(number => number.toString(16)) // Converts numbers to hex
        .map(string => (string.length === 1 ? `0${string}` : string)) // Adds 0 when length of one number is 1
        .join('')}`
  )
}
/**
 * Assigns visually distinct, high-contrast colors to employees.
 * Uses a static palette for the first N, then generates HSL colors for overflow.
 * @param employees Array of employee IDs or names
 * @returns Record mapping employee to hex color
 */
export function assignEmployeeColors(employees: string[]): Record<string, string> {
  // Static palette: 12 visually distinct, high-contrast colors (from D3/ColorBrewer)
  // 20-color, colorblind-safe, high-contrast palette (Paul Tol + Okabe-Ito)
  const palette = [
    "#4477AA", // blue
    "#EE6677", // red
    "#228833", // green
    "#CCBB44", // yellow
    "#66CCEE", // sky blue
    "#AA3377", // purple
    "#BBBBBB", // gray
    "#0099BB", // cyan
    "#EE7733", // orange
    "#0077BB", // dark blue
    "#33BBEE", // light blue
    "#EE3377", // magenta
    "#009988", // teal
    "#EEAABB", // pink
    "#44AA99", // turquoise
    "#999933", // olive
    "#DDCC77", // sand
    "#117733", // dark green
    "#332288", // indigo
    "#88CCEE", // pale blue
  ];

  // Helper: HSL to hex
  function hslToHex(h: number, s: number, l: number): string {
    s /= 100;
    l /= 100;
    const k = (n: number) => (n + h / 30) % 12;
    const a = s * Math.min(l, 1 - l);
    const f = (n: number) =>
      l - a * Math.max(-1, Math.min(Math.min(k(n) - 3, 9 - k(n)), 1));
    const toHex = (x: number) => {
      const hex = Math.round(x * 255).toString(16);
      return hex.length === 1 ? "0" + hex : hex;
    };
    return `#${toHex(f(0))}${toHex(f(8))}${toHex(f(4))}`;
  }

  const colorMap: Record<string, string> = {};
  const usedColors: string[] = [];

  employees.forEach((employee, idx) => {
    let color: string;
    if (idx < palette.length) {
      color = palette[idx];
    } else {
      // Improved: Generate new color with high contrast and separation
      // Use only hues that are not too close to palette, avoid yellow (faint), avoid too dark/light
      let hue = (idx * 53) % 360; // 53 for better separation
      let sat = 75; // higher saturation for vibrancy
      let light = 65; // avoid too dark/light
      color = hslToHex(hue, sat, light);

      // Ensure not too close to previous colors and high contrast with white
      let attempts = 0;
      while (
        (
          usedColors.some(
            c => chromaDistance(color, c) < 80 // stricter threshold
          ) ||
          contrastWithWhite(color) < 4.5 // WCAG AA minimum
        ) &&
        attempts < 12
      ) {
        // Try next hue and tweak saturation/lightness
        hue = (hue + 37) % 360;
        sat = 70 + (attempts % 2) * 10;
        light = 60 + (attempts % 3) * 5;
        color = hslToHex(hue, sat, light);
        attempts++;
      }
    }
    colorMap[employee] = color;
    usedColors.push(color);
  });

  return colorMap;
}

// Contrast ratio with white (returns value >=1)
function contrastWithWhite(hex: string): number {
  const rgb = hexToRgbArr(hex);
  if (!rgb) return 1;
  // Relative luminance
  const lum = (c: number) => {
    c /= 255;
    return c <= 0.03928
      ? c / 12.92
      : Math.pow((c + 0.055) / 1.055, 2.4);
  };
  const L1 = 1; // white
  const L2 = 0.2126 * lum(rgb[0]) + 0.7152 * lum(rgb[1]) + 0.0722 * lum(rgb[2]);
  return (L1 + 0.05) / (L2 + 0.05);
}

// Simple color distance in RGB space
function chromaDistance(hex1: string, hex2: string): number {
  const rgb1 = hexToRgbArr(hex1);
  const rgb2 = hexToRgbArr(hex2);
  if (!rgb1 || !rgb2) return 999;
  return Math.sqrt(
    Math.pow(rgb1[0] - rgb2[0], 2) +
      Math.pow(rgb1[1] - rgb2[1], 2) +
      Math.pow(rgb1[2] - rgb2[2], 2)
  );
}

// Helper: hex to [r,g,b]
function hexToRgbArr(hex: string): [number, number, number] | null {
  hex = hex.replace(/^#/, "");
  if (hex.length === 3) {
    hex = hex
      .split("")
      .map(x => x + x)
      .join("");
  }
  if (hex.length !== 6) return null;
  return [
    parseInt(hex.slice(0, 2), 16),
    parseInt(hex.slice(2, 4), 16),
    parseInt(hex.slice(4, 6), 16),
  ];
}
