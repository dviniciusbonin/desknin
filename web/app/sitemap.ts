import type { MetadataRoute } from "next";
import { locales } from "@/lib/i18n";
import { getSiteUrl } from "@/lib/site";

export const dynamic = "force-static";

const paths = ["", "/onboarding", "/privacy", "/terms"] as const;

export default function sitemap(): MetadataRoute.Sitemap {
  const siteUrl = getSiteUrl();
  const lastModified = new Date();

  return locales.flatMap((locale) =>
    paths.map((path) => ({
      url: `${siteUrl}/${locale}${path}`,
      lastModified,
      changeFrequency: (path === "" ? "weekly" : "monthly") as "weekly" | "monthly",
      priority: path === "" ? 1 : path === "/onboarding" ? 0.8 : 0.4,
      alternates: {
        languages: Object.fromEntries(
          locales.map((item) => [item, `${siteUrl}/${item}${path}`]),
        ),
      },
    })),
  );
}
