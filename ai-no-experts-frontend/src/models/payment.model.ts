export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    country: string;
    email: string;
    fullName: string;
    phone: string;
    // Sensitive information should be handled securely and not stored in plain text
    // cardNumber: string;
    // cvv: string;
    // expirationDate: string;
    // ssn: string;
}