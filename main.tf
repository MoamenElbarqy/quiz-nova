resource "azurerm_resource_group" "app_rg" {
  name     = "quiz-nova-resource-group"
  location = "swedencentral"
}

resource "azurerm_container_app_environment" "app_env" {
  name                = "quiznova-env"
  resource_group_name = azurerm_resource_group.app_rg.name
  location            = azurerm_resource_group.app_rg.location
}

resource "azurerm_container_app" "backend_app" {
  name                         = "quiznova-api"
  container_app_environment_id = azurerm_container_app_environment.app_env.id
  resource_group_name          = azurerm_resource_group.app_rg.name
  revision_mode                = "Single"
  workload_profile_name        = "Consumption"

  template {

    min_replicas = 0
    max_replicas = 1

    container {
      name   = "quiznova-api"
      image  = "ghcr.io/moamenelbarqy/quiz-nova-api:latest"
      cpu    = "0.5"
      memory = "1Gi"

      env {
        name  = "AppSettings__Cors__AllowedOrigins__0"
        value = "https://moamenelbarqy.github.io"
      }
      env {
        name  = "AppSettings__Cors__AllowedOrigins__1"
        value = "https://quiznova.dev"
      }
      env {
        name  = "AppSettings__Cors__AllowedOrigins__2"
        value = "https://www.quiznova.dev"
      }
      env {
        name  = "JwtSettings__Issuer"
        value = "https://quiznova-api.purpleforest-454b82e9.swedencentral.azurecontainerapps.io"
      }
      env {
        name  = "JwtSettings__Audiences__0"
        value = "https://moamenelbarqy.github.io"
      }
      env {
        name  = "JwtSettings__Audiences__1"
        value = "https://quiznova.dev"
      }
      env {
        name  = "JwtSettings__Audiences__2"
        value = "https://www.quiznova.dev"
      }


      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }
      env {
        name  = "AutoMigrateDb"
        value = "true"
      }
      env {
        name  = "ConnectionStrings__DefaultConnection"
        value = var.db_connection_string
      }
      env {
        name  = "SERILOG__USING__0"
        value = "Serilog.Sinks.Grafana.Loki"
      }
      env {
        name  = "SERILOG__WRITETO__1__NAME"
        value = "GrafanaLoki"
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__URI"
        value = var.grafana_loki_uri
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__LABELS__0__KEY"
        value = "app"
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__LABELS__0__VALUE"
        value = "QuizNova.Api"
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__LABELS__1__KEY"
        value = "env"
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__LABELS__1__VALUE"
        value = "production"
      }
      env {
        name  = "JwtSettings__Secret"
        value = var.jwt_secret
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__CREDENTIALS__LOGIN"
        value = "1640218"
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__CREDENTIALS__PASSWORD"
        value = var.grafana_loki_password
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__PERIOD"
        value = "00:00:01"
      }
      env {
        name  = "SERILOG__WRITETO__1__ARGS__EAGERLYEMITFIRSTEVENT"
        value = "true"
      }
      env {
        name  = "OTEL_SERVICE_NAME"
        value = "QuizNova.Api"
      }
      env {
        name  = "OTEL_EXPORTER_OTLP_ENDPOINT"
        value = var.grafana_otlp_endpoint
      }
      env {
        name  = "OTEL_EXPORTER_OTLP_HEADERS"
        value = "Authorization=Basic ${var.grafana_otlp_auth_header}"
      }
      env {
        name  = "OTEL_EXPORTER_OTLP_PROTOCOL"
        value = "http/protobuf"
      }
    }
  }

  ingress {
    allow_insecure_connections = false
    external_enabled           = true
    target_port                = 8080
    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  # CRITICAL: Ignore changes to application configurations that are 
  # managed by GitHub Actions, Azure Portal, or contain sensitive secrets.
  lifecycle {
    ignore_changes = [
      template[0].container[0].image,
      template[0].container[0].liveness_probe,
      template[0].container[0].readiness_probe,
      template[0].container[0].startup_probe,
      secret,
      registry
    ]
  }
}


resource "github_repository_pages" "quiz_nova_pages" {
  repository = "quiz-nova"
  cname      = "quiznova.dev"
  source {
    branch = "gh-pages"
    path   = "/"
  }
}

