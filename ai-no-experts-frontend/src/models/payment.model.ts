export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    cardNumber: string; // Consider using a different type to store cardNumber securely, e.g., as an encrypted string or token
    country: string;
    cvv: string; // Consider removing or using a tokenized value for cvv due to security concerns
    email: string;
    expirationDate: string; // Consider using a Date type or another method to ensure proper handling
    fullName: string;
    phone: string;
    ssn: string; // Sensitive information, consider using encryption or masking
}