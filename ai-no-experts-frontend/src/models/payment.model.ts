export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    cardNumber: string; // Ensure to mask or encrypt this field
    country: string;
    cvv: string; // Security best practice: tokenize or securely handle this field
    email: string;
    expirationDate: string;
    fullName: string;
    phone: string;
    ssn: string; // Highly sensitive, ensure encryption and handle according to data protection regulations
};

// Suggested to implement proper encryption and decryption methods as per compliance standards (e.g., PCI DSS)