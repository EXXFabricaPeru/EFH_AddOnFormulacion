using AddonListaMaterialesYrutas.commons;
using System;

namespace AddonListaMaterialesYrutas.models
{
    class Lote
    {
        public int MatrixRow;
        public readonly string LotCode;
        public readonly DateTime DueDate;
        public readonly DateTime ExpirationDate;
        public readonly DateTime CreationDate;
        public readonly double AvailableQuantity;
        public readonly double AssignedQuantity;
        public readonly int DaysToExpire;
        public double SelectedQuantity { private set; get; }
        public double PickableQuantity { private set; get; }

        public Lote(int matrixRow, string lotCode, string dueDate, string expirationDate, string createDate, string availableQuantity, string assignedQuantity)
        {
            MatrixRow = matrixRow;
            LotCode = lotCode;
            DueDate = DateTime.ParseExact(dueDate, Constantes.SAPDATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            ExpirationDate = string.IsNullOrEmpty(expirationDate) ? DateTime.MaxValue : DateTime.ParseExact(expirationDate, Constantes.SAPDATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            CreationDate = DateTime.ParseExact(createDate, Constantes.SAPDATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            AssignedQuantity = Double.Parse(assignedQuantity);
            AvailableQuantity = Double.Parse(availableQuantity);
            DaysToExpire = (ExpirationDate.Date - DueDate.Date).Days;
            SelectedQuantity = 0.0d;
            PickableQuantity = AvailableQuantity - AssignedQuantity;
        }

    }
}
