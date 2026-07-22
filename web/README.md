# DeskNin Web

DeskNin Web is the marketing and onboarding frontend for DeskNin, built with Next.js.

The goal of this project is to provide a localized landing site, self-serve workspace signup, and auth entry points that sit in front of the DeskNin help desk platform.

---

## Features

- Localized landing page (`en-us`, `pt-br`, `es-es`)
- Light / dark theme
- Plans and pricing (trial, Standard, unlimited)
- Self-serve onboarding
- Login and forgot-password flows
- WhatsApp contact CTAs
- SEO (metadata, robots, sitemap, JSON-LD)
- Static landing generation (SSG)

---

## Tech Stack

<p>
  <img src="https://skillicons.dev/icons?i=nextjs,react,ts,tailwind" />
</p>

---

## Getting started

```bash
cd web
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) (redirects to a locale such as `/en-us`).

### Environment

```bash
NEXT_PUBLIC_SITE_URL=http://localhost:3000   # public web origin (canonical / OG / sitemap)
APP_API_URL=http://localhost:8080            # optional backend base URL
```

Use the production site URL for `NEXT_PUBLIC_SITE_URL` when deploying so search engines index the correct origin.

If `APP_API_URL` is set, forms post to:

- `POST /api/onboarding`
- `POST /api/auth/login`
- `POST /api/auth/forgot-password`

Otherwise forms validate and succeed locally until the multi-tenant API is available.

### Scripts

- `npm run dev` — development server
- `npm run build` — production build
- `npm run start` — serve production build
- `npm run lint` — ESLint

### Docker

```bash
cd web
docker compose up --build
```

Open [http://localhost:8080](http://localhost:8080). The image uses a multi-stage build with Next.js `output: "standalone"`.

---

## Author

[Douglas Vinicius Caldas Bonin](https://github.com/dviniciusbonin)
