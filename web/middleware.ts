import { NextRequest, NextResponse } from "next/server";
import { defaultLocale, isLocale, locales, type Locale } from "@/lib/i18n";

function negotiateLocale(header: string | null): Locale {
  if (!header) {
    return defaultLocale;
  }

  const candidates = header.split(",").map((part) => {
    const [tag, ...params] = part.trim().split(";");
    const q = params.find((param) => param.trim().startsWith("q="));
    const quality = q ? Number(q.trim().slice(2)) : 1;
    return { tag: tag.toLowerCase(), quality: Number.isFinite(quality) ? quality : 0 };
  });

  candidates.sort((a, b) => b.quality - a.quality);

  for (const { tag } of candidates) {
    const exact = locales.find((locale) => locale === tag);
    if (exact) {
      return exact;
    }

    const prefix = tag.split("-")[0];
    const byPrefix = locales.find((locale) => locale.startsWith(`${prefix}-`));
    if (byPrefix) {
      return byPrefix;
    }
  }

  return defaultLocale;
}

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (
    pathname.startsWith("/_next") ||
    pathname.startsWith("/api") ||
    pathname.includes(".") ||
    pathname === "/robots.txt" ||
    pathname === "/sitemap.xml"
  ) {
    return NextResponse.next();
  }

  const segment = pathname.split("/")[1];
  if (segment) {
    const normalized = segment.toLowerCase();
    if (isLocale(normalized)) {
      if (segment !== normalized) {
        const url = request.nextUrl.clone();
        url.pathname = `/${normalized}${pathname.slice(segment.length + 1)}`;
        return NextResponse.redirect(url);
      }
      return NextResponse.next();
    }
  }

  const matched = negotiateLocale(request.headers.get("accept-language"));
  const url = request.nextUrl.clone();
  url.pathname = `/${matched}${pathname === "/" ? "" : pathname}`;
  return NextResponse.redirect(url);
}

export const config = {
  matcher: ["/((?!_next/static|_next/image|favicon.ico|.*\\..*).*)"],
};
