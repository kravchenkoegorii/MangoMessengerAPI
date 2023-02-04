variable "resource_group_name" {
  default     = "rg-messenger-d04"
  description = "Prefix of the resource group name that's combined with a random ID so name is unique in your Azure subscription."
}

variable "resource_group_location" {
  default     = "westus"
  description = "Location of the resource group."
}

variable "app_service_plan_name" {
  default     = "messenger-plan-d01"
  description = "Messenger app service plan name"
}

variable "app_service_name" {
  default     = "app-messenger-d01"
  description = "Messenger app service name"
}

variable "storage_account_name" {
  default     = "mangostoraged01"
  description = "Messenger storage account name"
}

variable "storage_account_tier" {
  default     = "Standard"
  description = "Messenger storage account tier"
}

variable "storage_account_replication" {
  default     = "LRS"
  description = "Messenger storage account replication strategy"
}

variable "storage_container_name" {
  default     = "cont-messenger"
  description = "Messenger storage container name"
}

variable "sql_server_name" {
  default     = "mango-sql-db-d01"
  description = "Messenger sql server name"
}

variable "sql_database_name" {
  default     = "mango-dev"
  description = "Messenger sql database name"
}

variable "sql_admin_username" {
  type        = string
  description = "Sql admin username"
}

variable "sql_admin_password" {
  type        = string
  description = "Sql admin password"
}