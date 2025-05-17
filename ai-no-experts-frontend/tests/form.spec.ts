import { test, expect } from '@playwright/test';

test('submits form with valid data', async ({page}) => {
  await page.goto('http://localhost:5175/');
  await page.fill('input[name="fullName"]', 'John Doe');
  await page.fill('input[name="email"]', 'john@example.com');
  await page.fill('input[name="cardNumber"]', '4111111111111111');
  await page.fill('input[name="expirationDate"]', '12/25');
  await page.fill('input[name="cvv"]', '123');
  await page.fill('input[name="billingAddress"]', '123 Main St');
  await page.click('button[type="submit"]');
  await expect(page.locator('form')).toBeVisible();
});

test('shows validation errors for empty required fields', async ({page}) => {
  await page.goto('http://localhost:5175/');
  await page.click('button[type="submit"]');
  await expect(page.locator('input[name="fullName"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="email"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="cardNumber"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="expirationDate"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="cvv"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="billingAddress"]')).toHaveAttribute('aria-invalid', 'true');
});

test('resets form when clicking reset button', async ({page}) => {
  await page.goto('http://localhost:5175/');
  await page.fill('input[name="fullName"]', 'John Doe');
  await page.fill('input[name="email"]', 'john@example.com');
  await page.click('button:text("Reset")');
  await expect(page.locator('input[name="fullName"]')).toHaveValue('');
  await expect(page.locator('input[name="email"]')).toHaveValue('');
});

test('validates email format', async ({page}) => {
  await page.goto('http://localhost:5175/');
  await page.fill('input[name="email"]', 'invalid-email');
  await page.click('button[type="submit"]');
  await expect(page.locator('input[name="email"]')).toHaveAttribute('aria-invalid', 'true');
});

