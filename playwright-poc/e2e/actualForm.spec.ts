import { test, expect } from '@playwright/test';
import { formData } from './utils/formData';

test('Llenar formulario actual con fricción', async ({ page }) => {
  await page.goto('http://localhost:3000/form');

  await page.fill('#input-name', formData.name); // campos actuales, tal vez poco amigables
  await page.fill('#input-email', formData.email);
  await page.fill('#input-message', formData.feedback);

  await page.click('button[type=submit]');
  await expect(page.locator('.success-message')).toContainText('Gracias');

  // Simular delay/fricción
  await page.waitForTimeout(500); // ejemplo para medición de performance
});