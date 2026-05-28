# 🚀 Azure Hosting & Budget Optimization Guide
## QuizNova (.NET 10 + Angular 21 + PostgreSQL 18)

This guide provides a comprehensive deployment strategy to host **QuizNova** on Microsoft Azure. Since this is a **VC/pitch project** and you have a **$200 Azure credit**, our absolute priority is **budget preservation**.

By selecting the right services, we can host this entire stack with **$0.00 / month** in ongoing running costs, making your $200 credit last indefinitely and shielding you from surprise credit card bills.

---

## 📊 Architectural Options & Pricing Comparison

Azure offers multiple ways to host a .NET 10 API, an Angular frontend, and a PostgreSQL database. Below is the cost breakdown for the three best options.

| Metric | 🌲 Option 1: Serverless Free-Tier (Recommended) | ☁️ Option 2: 12-Month Azure Free Services | 🖥️ Option 3: All-in-One VM Docker Compose |
| :--- | :--- | :--- | :--- |
| **Frontend** | Azure Static Web Apps (Free Tier) | Azure Static Web Apps (Free Tier) | Self-hosted inside VM |
| **Backend** | Azure Container Apps (Scale to Zero) | Azure App Service Linux (Basic B1 Plan) | Self-hosted inside VM |
| **Database** | Serverless PostgreSQL (Neon.tech / Supabase) | Azure PostgreSQL Flexible Server (12-Mo Free) | PostgreSQL in Docker Container |
| **Observability** | External Free SaaS (Honeycomb / Grafana Cloud) | Application Insights (Basic Free Tier) | Seq, Prometheus, Grafana in Docker |
| **Ongoing Cost** | **$0.00 / month** (Permanent) | **$0.00 / month** (Year 1), then **~$30/month** | **$0.00 / month** (Year 1 on B1s), then **~$5/month** |
| **$200 Lifespan** | **Indefinite (Years)** | **~18.5 Months Total** (12m Free + 6.5m Credit) | **~12 Months (B2s VM)** / **Years (B1s VM)** |
| **Cold Starts** | Yes (5-8 seconds on first api call) | No | No |
| **Scaling** | Automatically scales up for pitch traffic | Manual scaling | Fixed limits (vertical scaling only) |
| **Setup Effort** | Medium | Easy | Hard (Docker installation, Nginx, Let's Encrypt) |

---

## 🌲 Option 1: Serverless & Permanent Free-Tier (Strongly Recommended)

This architecture uses serverless components that scale to zero when there is no traffic (e.g. between pitch meetings). When idle, **it consumes $0.00**.

```
                      ┌────────────────────────┐
                      │    Angular Frontend    │
                      │ Azure Static Web Apps  │ (CDN - $0/month)
                      └───────────┬────────────┘
                                  │
                           HTTPS (API Calls)
                                  │
                                  ▼
                      ┌────────────────────────┐
                      │     .NET 10 API        │
                      │  Azure Container Apps  │ (Scale-to-Zero - $0/month)
                      └───────────┬────────────┘
                                  │
                          EF Core Connection
                                  │
                                  ▼
                      ┌────────────────────────┐
                      │  Serverless Postgres   │
                      │  Neon.tech / Supabase  │ (Managed - $0/month)
                      └────────────────────────┘
```

### 1. Frontend (Angular 21): Azure Static Web Apps (SWA)
* **Cost**: **$0 / month** (Permanent Free Plan).
* **Why**: SWA hosts static assets directly on global CDNs. It is faster than hosting inside a Docker container, comes with free custom domains and automatic SSL, and has **zero overhead**.

### 2. Backend (.NET 10): Azure Container Apps (ACA)
* **Cost**: **$0 / month** (Consumption Plan).
* **Why**: ACA offers a permanent free allowance of **180k vCPU-seconds and 360k GiB-seconds** per month.
  * If we allocate **0.25 vCPU and 0.5 GiB RAM** to your API container, it can run continuously for **200 hours active** every month completely free.
  * By configuring **scale-to-zero**, the container shuts down when idle. When a investor visits the site, it spins up on-demand.
* **Cost Hack (Docker Registry)**: Azure Container Registry (ACR) costs ~$5/month. To bypass this, we use **GitHub Container Registry (GHCR.io)** (100% free) to store your Docker images, which ACA pulls directly.

### 3. Database (PostgreSQL 18): Serverless Neon.tech or Supabase
* **Cost**: **$0 / month** (Free Plan).
* **Why**: Azure's managed PostgreSQL database starts at $15/month and will quickly eat your $200 credit after 12 months.
  * **Neon.tech** offers a fully-managed serverless PostgreSQL database with 0.5 GB storage, autoscaling, and auto-sleep.
  * **Supabase** offers a free 500 MB PostgreSQL database.
  * Both support standard connection strings compatible with EF Core.

---

## 🛠️ Step-by-Step Setup Guide (Option 1)

### Step 1: Provision the Serverless Database
1. Go to [Neon.tech](https://neon.tech/) (or Supabase) and create a free account.
2. Spin up a new PostgreSQL database and choose your region (e.g., East US or West Europe close to your target audience).
3. Copy the connection string. It will look like this:
   `Host=ep-cool-water-12345.us-east-2.aws.neon.tech;Database=neondb;Username=moamen;Password=your_secret_password;SSL Mode=Require;Trust Server Certificate=true`

### Step 2: Code Adjustments for Production Auto-Migration
Currently, your `Program.cs` only runs EF migrations in development. We want the container to migrate our Neon database automatically on startup if a flag is passed, saving you from running EF commands manually in production.

We can add an environment variable check in `src/QuizNova.Api/Program.cs`:

```csharp
// Replace line 36-39:
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("AutoMigrateDb"))
{
    await app.Services.InitializeDevelopmentDatabaseAsync();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        // ... Swagger UI Setup ...
    }
}
```

This way, when deploying the API container, you simply set `AutoMigrateDb=true` as an environment variable, and the database schema will instantly sync and seed on boot!

### Step 3: Publish the API Image to GitHub Container Registry (GHCR)
Instead of paying for Azure Container Registry (ACR), you can build your Docker image via GitHub Actions and push it to GHCR.

Create a GitHub workflow file `.github/workflows/deploy-backend.yml`:
```yaml
name: Deploy Backend

on:
  push:
    branches: [ "main" ]
    paths:
      - "src/QuizNova.Api/**"
      - "src/QuizNova.Application/**"
      - "src/QuizNova.Domain/**"
      - "src/QuizNova.Infrastructure/**"
      - "Directory.Build.props"
      - "Directory.Packages.props"

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - name: Checkout Repository
        uses: actions/checkout@v4

      - name: Log in to GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Downcase Repo Name
        run: echo "IMAGE_NAME=$(echo ${{ github.repository }} | tr '[:upper:]' '[:lower:]')" >> $GITHUB_ENV

      - name: Build and Push Docker Image
        uses: docker/build-push-action@v6
        with:
          context: .
          file: src/QuizNova.Api/Dockerfile
          push: true
          tags: ghcr.io/${{ env.IMAGE_NAME }}-api:latest
```

### Step 4: Deploy the Backend to Azure Container Apps (ACA)
1. In the **Azure Portal**, search for **Container Apps** and click **Create**.
2. Select your subscription (which has the $200 credit) and create a Resource Group (e.g., `QuizNova-RG`).
3. Under **Container App Details**, name your app (e.g., `quiznova-api`).
4. Set **Deployment source** to **Container image**.
5. Select **Other Image Registry**:
   * **Registry login server**: `ghcr.io`
   * **Registry username**: Your GitHub username.
   * **Registry password**: A GitHub Personal Access Token (PAT) with `read:packages` permissions.
   * **Image and tag**: `ghcr.io/<your-github-username>/quiznova-api:latest`
6. Under **Application Scaling and CPU**:
   * Choose **Consumption Plan**.
   * Set CPU to `0.25 Cores` and Memory to `0.5 GiB` (minimum allowed, highly cost-effective).
7. Under **Ingress**:
   * Enable Ingress.
   * Set Ingress traffic to **Limited to VNet** or **Accepting traffic from anywhere** (External). Since Angular needs to talk to the API, select **External** (Accepting traffic from anywhere).
   * Set **Target Port** to `8080` (as exposed in the API Dockerfile).
8. Go to the **Configuration > Secrets** tab in your Container App and add a secret named `db-connection-string` containing your Neon connection string.
9. Go to the **Configuration > Containers** tab and configure your Environment Variables:
   * `ConnectionStrings__DefaultConnection` ➔ Referencing the secret `db-connection-string`
   * `ASPNETCORE_ENVIRONMENT` ➔ `Production`
   * `AutoMigrateDb` ➔ `true`
   * `JwtSettings__Secret` ➔ `<A-Long-Secure-Random-Key-For-Prod>`
   * `JwtSettings__Issuer` ➔ `quiznova-api.<azure-domain>`
   * `JwtSettings__Audience` ➔ `quiznova-client.<azure-domain>`
10. Go to the **Scale** tab:
    * Set **Min Replicas** to `0` (This enables scale-to-zero and ensures $0/month cost when idle!).
    * Set **Max Replicas** to `5`.
11. Click **Review + Create**. Once created, Azure will give you a public URL for your API (e.g. `https://quiznova-api.bluecliff-12345.eastus.azurecontainerapps.io`).

### Step 5: Configure the Frontend Production Environment
1. Open `src/QuizNova.Client/src/environments/environment.production.ts`.
2. Update the `apiUrl` to point to your new Azure Container App URL:
   ```typescript
   export const environment = {
     appName: 'QuizNova',
     isProduction: true,
     apiUrl: 'https://quiznova-api.bluecliff-12345.eastus.azurecontainerapps.io/', // Update this!
     enableDevTools: false,
   };
   ```
3. Commit and push this change to your repository.

### Step 6: Deploy Angular to Azure Static Web Apps (SWA)
1. In the **Azure Portal**, search for **Static Web Apps** and click **Create**.
2. Select your subscription, Resource Group (`QuizNova-RG`), and name your app (e.g., `quiznova-client`).
3. Select the **Free Plan** ($0/month).
4. Choose **GitHub** as the deployment source and log in.
5. Select your organization, repository, and branch (`main`).
6. Under **Build Details**, select **Angular** as the preset:
   * **App location**: `/src/QuizNova.Client` (The subfolder holding the frontend project).
   * **Api location**: Leave empty (since we host our API separately in ACA).
   * **Output location**: `dist/quiz-nova-client/browser` (This is where Angular 21 outputs built browser assets, as confirmed in your `angular.json`).
7. Click **Review + Create**.
8. SWA will automatically add a GitHub Actions workflow `.github/workflows/azure-static-web-apps-xxxx.yml` to your repository. The action will run immediately, build your Angular project in production mode (replacing the environment file with `environment.production.ts`), and publish it globally.

---

## 📈 Observability & Monitoring in Production

In your local `compose.yaml`, you are running **Seq, Prometheus, and Grafana**.

> [!WARNING]
> **Do not deploy Seq, Prometheus, and Grafana containers to Azure Container Apps.**
> Running these containers continuously will cost resources, completely bypass scale-to-zero (they require active disks and run continuously), and exhaust your $200 credit in a couple of months.

### The Low-Cost Observability Plan
Since you are pitching to VCs, you still want logging and metrics, but at **$0 cost**.

1. **Logging (Serilog)**:
   * Configure Serilog to output to **Console (stdout)** in production. Azure Container Apps automatically scrapes stdout and lets you inspect logs for free in the **Log Stream** tab of the portal.
   * Alternatively, ship logs to **Seq Cloud** (free tier supports limited ingestion) or **Axiom.co** (free plan provides 50 GB of logs/month).
2. **Metrics & Tracing (OpenTelemetry)**:
   * Turn off OTLP export to local Seq, or redirect the OpenTelemetry export to a free external APM provider:
     * **Honeycomb.io (Free Plan)**: Offers 20 million spans/month. Perfect for deep tracing of C# requests.
     * **Grafana Cloud (Free Plan)**: Offers 3 users, 10,000 metrics, 50 GB logs, and 50 GB traces completely free. You can stream OpenTelemetry metrics there and get premium dashboards without paying a cent.

---

## 🛡️ Top Budget-Saving Best Practices

1. **Set Up Budget Alerts**: Go to **Azure Portal > Cost Management + Billing > Budgets** and create a budget of **$10/month** or **$50 total**. Have it send an email to you immediately if usage climbs above $2, protecting you from misconfigurations.
2. **Setup Auto-Sleep/Scale-to-Zero on Neon**: Ensure the auto-suspend time on your Neon.tech DB is set to **5 minutes** of inactivity to keep your database usage 100% free.
3. **Use GitHub Pages or Cloudflare Pages as a SWA Alternative**: If Azure SWA ever starts charging or runs out of bandwidth, Cloudflare Pages is 100% free with unlimited bandwidth, making it an excellent fallback for your static Angular frontend.
4. **Scale Down VM (If using Option 3)**: If you decide to go with Option 3 (a single virtual machine), remember to use a **B1s** instance (free for 12 months, then $5/month) and keep memory consumption tight. Disable Grafana, Seq, and Prometheus to prevent the VM from locking up and swapping.

---

### 🎉 Summary of Monthly Running Cost
* **Angular Client (SWA)**: $0.00
* **.NET 10 API (ACA with Scale-to-Zero)**: $0.00 (under free allowance)
* **PostgreSQL (Neon Serverless)**: $0.00
* **Docker Registry (GHCR)**: $0.00
* **APM Traces (Honeycomb/Grafana Cloud)**: $0.00
* **Grand Total**: **$0.00 / month** 💸

Your $200 Azure credit remains completely untouched and is reserved as a safety net!
