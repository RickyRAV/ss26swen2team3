# Build context must be the repo root (not apps/web) because pnpm-lock.yaml lives there.
# docker-compose files use:  context: .  |  dockerfile: apps/web/Dockerfile

# ─── Base ─────────────────────────────────────────────────────────────────────
FROM node:22-alpine AS base
WORKDIR /repo
RUN npm install -g pnpm@10.28.2

# ─── Install dependencies ─────────────────────────────────────────────────────
FROM base AS deps
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
COPY apps/web/package.json apps/web/package.json
RUN pnpm install --frozen-lockfile --filter=./apps/web...

# ─── Development stage ────────────────────────────────────────────────────────
FROM deps AS development
WORKDIR /repo/apps/web
EXPOSE 5173
CMD ["pnpm", "dev", "--host", "0.0.0.0"]

# ─── Build stage ──────────────────────────────────────────────────────────────
FROM deps AS build
COPY apps/web ./apps/web
WORKDIR /repo/apps/web
ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
RUN pnpm build

# ─── Production stage ─────────────────────────────────────────────────────────
FROM nginx:alpine AS production
COPY --from=build /repo/apps/web/dist /usr/share/nginx/html
COPY apps/web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
