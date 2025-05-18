import { FormControl, InputAdornment, TextField } from '@mui/material';
import { Field, ErrorMessage } from 'formik';
import { type FunctionComponent, useMemo } from 'react';

const FormField: FunctionComponent<FormFieldProps> = ({
    endAdornment,    
    fullWidth = true,
    label = '',
    name = '',
    startAdornment,
    type = 'text',
    error = false,
    ...props
}) => {
    // useMemo hook to avoid unnecessary recomputation of InputAdornments
    const startAdornmentComponent = useMemo(() => startAdornment ? <InputAdornment position="start">{startAdornment}</InputAdornment> : null, [startAdornment]);
    const endAdornmentComponent = useMemo(() => endAdornment ? <InputAdornment position="end">{endAdornment}</InputAdornment> : null, [endAdornment]);

    return (
        <FormControl fullWidth={fullWidth} margin="normal">
            <Field
                as={TextField}
                fullWidth={fullWidth}
                id={name}
                name={name}
                label={label}
                type={type}
                error={error}
                InputProps={{
                    startAdornment: startAdornmentComponent,
                    endAdornment: endAdornmentComponent,
                }}
                {...props}
            />
            <ErrorMessage className='form-error' name={name} component="p" />
        </FormControl>
    );
}

type FormFieldProps = {
    endAdornment?: string;
    error?: boolean;
    fullWidth?: boolean,
    label: string;
    name: string;
    startAdornment?: string;
    type?: string;
    [key: string]: any;
}

export default FormField;