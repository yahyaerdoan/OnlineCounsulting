// Edge Worker for tenant custom domains (Cloudflare for SaaS).
// Forwards each request to the web app and tells it the customer's domain.
// Variables: ORIGIN_HOST (e.g. "comfortxpro.up.railway.app"), EDGE_SECRET (same value as the web app's Edge__Secret).
// Moving servers only changes ORIGIN_HOST; customers' DNS stays the same.
export default {
  async fetch(request, env) {
    const url = new URL(request.url);
    const customerHost = url.hostname;
    url.hostname = env.ORIGIN_HOST;
    url.protocol = "https:";
    url.port = "";

    // Copies method, body and headers, including WebSocket upgrades for Blazor.
    const forwarded = new Request(url, request);
    forwarded.headers.set("X-Edge-Host", customerHost);
    forwarded.headers.set("X-Edge-Secret", env.EDGE_SECRET);

    return fetch(forwarded, { redirect: "manual" });
  },
};
