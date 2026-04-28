-- Initial migration placeholder for People module.
CREATE TABLE IF NOT EXISTS people_employees (
  id uuid PRIMARY KEY,
  full_name text,
  email text,
  status int
);
