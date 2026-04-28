-- Initial migration placeholder for Treasury module.
CREATE TABLE IF NOT EXISTS treasury_invoices (
  id uuid PRIMARY KEY,
  customer_id uuid,
  due_date date,
  amount numeric,
  status int
);
