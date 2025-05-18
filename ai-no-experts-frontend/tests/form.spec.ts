import { test, expect } from '@playwright/test';
import * as fs from 'fs';

test('submits form with valid data', async ({page}) => {
  await page.goto(' http://localhost:5173/');
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
  await page.goto(' http://localhost:5173/');
  await page.click('button[type="submit"]');
  await expect(page.locator('input[name="fullName"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="email"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="cardNumber"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="expirationDate"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="cvv"]')).toHaveAttribute('aria-invalid', 'true');
  await expect(page.locator('input[name="billingAddress"]')).toHaveAttribute('aria-invalid', 'true');
});

test('resets form when clicking reset button', async ({page}) => {
  await page.goto(' http://localhost:5173/');
  await page.fill('input[name="fullName"]', 'John Doe');
  await page.fill('input[name="email"]', 'john@example.com');
  await page.click('button:text("Reset")');
  await expect(page.locator('input[name="fullName"]')).toHaveValue('');
  await expect(page.locator('input[name="email"]')).toHaveValue('');
});

test('validates email format', async ({page}) => {
  await page.goto(' http://localhost:5173/');
  await page.fill('input[name="email"]', 'invalid-email');
  await page.click('button[type="submit"]');
  await expect(page.locator('input[name="email"]')).toHaveAttribute('aria-invalid', 'true');
});

test('checks accessibility features', async ({ page }) => {
  await page.goto('http://localhost:5173/');

  // Check for keyboard accessibility
  await page.keyboard.press('Tab');
  expect(await page.evaluate(() => document.activeElement?.getAttribute('name'))).toBe('fullName');

  // Check color contrast or hidden elements (using suitable libraries)
});

test('tests responsiveness across screen sizes', async ({ page }) => {
  const viewports = [
    { width: 1920, height: 1080 }, // Desktop
    { width: 768, height: 1024 }, // Tablet
    { width: 375, height: 812 },  // Mobile
  ];

  for (const viewport of viewports) {
    await page.setViewportSize(viewport);
    await page.goto('http://localhost:5173/');
    // Validate UI or specific responsiveness behavior
    await expect(page.locator('form')).toBeVisible();
  }
});

test('should measure performance metrics', async ({ page }) => {
  await page.goto(' http://localhost:5173/');

  const performanceMetrics = await page.evaluate(() => {
    const [navigationEntry] = performance.getEntriesByType('navigation');
    return JSON.stringify(navigationEntry.toJSON(), null, 2);
  });

  const metricsDir = 'tests/metrics';
  if (!fs.existsSync(metricsDir)) {
    fs.mkdirSync(metricsDir, {recursive: true});
  }

  fs.writeFileSync(`${metricsDir}/performance-metrics-${new Date().toISOString().replace(/:/g, '-')}.json`, performanceMetrics);
  console.log('Performance Metrics:', JSON.parse(performanceMetrics));
});