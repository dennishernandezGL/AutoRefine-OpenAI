export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    cardNumber: string; // Consider masking this in interfaces
    country: string;
    cvv: string; // Consider hashing or encrypting
    email: string;
    expirationDate: string; // Use a standardized date type
    fullName: string;
    phone: string; // Validate format
    ssn: string; // Consider masking or encrypting
}