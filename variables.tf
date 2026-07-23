variable "db_connection_string" {
  type        = string
  description = "Connection string for Neon PostgreSQL"
  sensitive   = true
}

variable "jwt_secret" {
  type        = string
  description = "JWT Signing Secret Key"
  sensitive   = true
}

variable "grafana_loki_password" {
  type        = string
  description = "Grafana Loki API Key / Password"
  sensitive   = true
}

variable "github_token" {
  type        = string
  description = "GitHub Personal Access Token with repo scope"
  sensitive   = true
}

variable "grafana_loki_uri" {
  description = "The Grafana Loki ingest endpoint URI"
  type        = string
}

variable "grafana_otlp_endpoint" {
  description = "Grafana Cloud OTLP Gateway endpoint for OpenTelemetry traces"
  type        = string
  default     = "https://otlp-gateway-prod-eu-west-2.grafana.net/otlp"
}

variable "grafana_otlp_auth_header" {
  description = "Base64 encoded Instance ID : API Key / Password for Grafana OTLP OTel traces"
  type        = string
  sensitive   = true
  default     = ""
}
