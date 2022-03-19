.PHONY: migrate_db
name="Migration-$${RANDOM}"
migrate_db:
	@dotnet ef migrations add $(name)

.PHONY: remore_migration
remove_migration:
	@dotnet ef migrations remove

.PHONY: update_db
update_db:
	@dotnet ef database update

.PHONY: run_movie_service
run_movie_service:
	@dotnet watch run
