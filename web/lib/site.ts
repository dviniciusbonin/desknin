export const siteConfig = {
  name: "DeskNin",
  contactEmail: "hello@desknin.com",
  /** E.164 digits without + — used for WhatsApp links */
  whatsappNumber: "5542998662656",
} as const;

export function getWhatsAppUrl(message?: string): string {
  const base = `https://wa.me/${siteConfig.whatsappNumber}`;
  if (!message) {
    return base;
  }
  return `${base}?text=${encodeURIComponent(message)}`;
}

const fallbackSiteUrl = "http://localhost:3000";

export function getSiteUrl(): string {
  return (process.env.NEXT_PUBLIC_SITE_URL ?? fallbackSiteUrl).replace(/\/$/, "");
}

export const planIds = ["trial", "standard", "unlimited"] as const;

export type PlanId = (typeof planIds)[number];

export const billingCycles = ["monthly", "annual"] as const;

export type BillingCycle = (typeof billingCycles)[number];

export function isBillingCycle(value: string): value is BillingCycle {
  return billingCycles.includes(value as BillingCycle);
}

/** Standard plan list prices by locale */
export const standardPricingByLocale = {
  "en-us": {
    currency: "USD",
    monthly: { amount: 5, label: "$5" },
    annual: { amount: 50, label: "$50" },
  },
  "es-es": {
    currency: "USD",
    monthly: { amount: 5, label: "$5" },
    annual: { amount: 50, label: "$50" },
  },
  "pt-br": {
    currency: "BRL",
    monthly: { amount: 29.9, label: "R$ 29,90" },
    annual: { amount: 299, label: "R$ 299" },
  },
} as const;

export const planMeta: Record<
  PlanId,
  { highlighted: boolean; ctaKind: "onboarding" | "contact"; priceCurrency?: string }
> = {
  trial: { highlighted: false, ctaKind: "onboarding", priceCurrency: "USD" },
  standard: { highlighted: true, ctaKind: "onboarding", priceCurrency: "USD" },
  unlimited: { highlighted: false, ctaKind: "contact" },
};
