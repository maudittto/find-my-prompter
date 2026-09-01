import type { NextConfig } from "next";

const apiUrl = process.env.API_URL ?? "http://localhost:5221";

const nextConfig: NextConfig = {
  // Proxy para a API: mantém os cookies do Identity na mesma origem do navegador,
  // evitando configuração de CORS/SameSite entre localhost:3000 e a API.
  async rewrites() {
    return [{ source: "/api/:path*", destination: `${apiUrl}/api/:path*` }];
  },
};

export default nextConfig;
