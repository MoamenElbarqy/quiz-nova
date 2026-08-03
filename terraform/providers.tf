terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.1.0"
    }
    github = {
      source  = "integrations/github"
      version = "~> 6.0"
    }
    mongodbatlas = {
      source  = "mongodb/mongodbatlas"
      version = "~> 2.15.0"
    }
  }
}

provider "azurerm" {
  subscription_id                 = "83ab56f5-88ee-436d-87a5-994d3185bf00"
  resource_provider_registrations = "none"
  features {}
}

provider "github" {
  # Authenticates using a GitHub Personal Access Token (PAT)
  token = var.github_token
  owner = "MoamenElbarqy"
}
provider "mongodbatlas" {
  public_key  = var.atlas_public_key
  private_key = var.atlas_private_key
}

data "azurerm_client_config" "current" {}
