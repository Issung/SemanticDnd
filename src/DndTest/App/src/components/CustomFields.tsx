import type { CustomField as CustomFieldModel } from "@/hooks/api/responses";

export function CustomFields({fields}: {fields: Array<CustomFieldModel>}) {
    return (
        <table className="CustomFields">
            {fields.map(f => <CustomField field={f}/>)}
        </table>
    );
}

function CustomField({field}: {field: CustomFieldModel}) {
    return (
        <tr className="CustomField">
            <td style={{paddingRight: 30}}>{field.name}</td>
            <td>{field.valueInteger ?? field.values.join(', ')}</td>
        </tr>
    );
}