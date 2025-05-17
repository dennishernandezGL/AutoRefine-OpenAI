import React, { useState } from 'react';
import './DualFormDemo.css'; // para estilos opcionales

export const DualFormDemo: React.FC = () => {
  const [submittedForm, setSubmittedForm] = useState<'actual' | 'improved' | null>(null);

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>, formType: 'actual' | 'improved') => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    console.log(`[${formType.toUpperCase()}] Datos enviados:`, Object.fromEntries(formData.entries()));
    setSubmittedForm(formType);
  };

  return (
    <div className="form-grid">
      {/* Formulario Actual */}
      <form onSubmit={(e) => handleSubmit(e, 'actual')} className="form-section">
        <h2>Formulario Actual</h2>
        <label>
          Nombre:
          <input id="input-name" name="name" type="text" placeholder="Nombre completo" />
        </label>
        <label>
          Correo:
          <input id="input-email" name="email" type="text" placeholder="correo@ejemplo.com" />
        </label>
        <label>
          Mensaje:
          <textarea id="input-message" name="feedback" placeholder="Tu mensaje..." />
        </label>
        <button type="submit">Enviar</button>
        {submittedForm === 'actual' && <p className="success-message">Gracias (formulario actual) ✅</p>}
      </form>

      {/* Formulario Mejorado por el LLM */}
      <form onSubmit={(e) => handleSubmit(e, 'improved')} className="form-section">
        <h2>Formulario Mejorado (LLM)</h2>
        <label>
          Full Name:
          <input type="text" placeholder="John Doe" name="name" />
        </label>
        <label>
          Work Email:
          <input type="email" placeholder="you@company.com" name="email" />
        </label>
        <label>
          Quick Feedback:
          <textarea placeholder="e.g., My issue is..." name="feedback" />
        </label>
        <button type="submit">Submit</button>
        {submittedForm === 'improved' && <p className="success-message">Gracias (mejorado por LLM) ✅</p>}
      </form>
    </div>
  );
};
