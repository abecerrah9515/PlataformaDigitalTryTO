-- Initial migration placeholder for Projects module.
CREATE TABLE IF NOT EXISTS projects_consolidation_executions (
  id uuid PRIMARY KEY,
  created_at timestamp,
  created_by text,
  updated_at timestamp,
  updated_by text,
  is_active boolean,
  status int
);
