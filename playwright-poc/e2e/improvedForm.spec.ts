import { test, expect } from '@playwright/test';
import { formData } from './utils/formData';
import llmSuggestions from '../llm_suggestions.json';

test('Llenar formulario mejorado (sugerencia del LLM)', async ({ page }) => {
  await page.goto('http://localhost:3000/form-improved');

  await page.fill(`input[placeholder="${llmSuggestions.name_field.placeholder}"]`, formData.name);
  await page.fill(`input[placeholder="${llmSuggestions.email_field.placeholder}"]`, formData.email);
  await page.fill(`textarea[placeholder="${llmSuggestions.feedback_field.placeholder}"]`, formData.feedback);

  await page.click('button[type=submit]');
  await expect(page.locator('.success-message')).toContainText('Gracias');

  // Comparar sin delay
});