// Scripts compartidos por todas las vistas del sistema.
(function ($) {
    'use strict';

    // jQuery Validate: aceptar números con coma decimal (cultura es-AR).
    if ($.validator) {
        const aNumero = function (valor) {
            return parseFloat(String(valor).replace(/\./g, '').replace(',', '.'));
        };

        $.validator.methods.number = function (valor, elemento) {
            return this.optional(elemento) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(valor);
        };

        $.validator.methods.range = function (valor, elemento, rango) {
            const numero = aNumero(valor);
            return this.optional(elemento) || (numero >= rango[0] && numero <= rango[1]);
        };
    }

    $(function () {
        // Listados con paginado, búsqueda y orden (DataTables).
        $('.datatable').DataTable({
            language: { url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json' },
            pageLength: 10,
            order: [],
            columnDefs: [{ targets: 'no-sort', orderable: false }]
        });

        // Confirmación de eliminación identificando el registro.
        $(document).on('submit', '.form-eliminar', function (evento) {
            evento.preventDefault();
            const formulario = this;
            const descripcion = $(formulario).data('descripcion');

            Swal.fire({
                title: 'Confirmar eliminación',
                text: '¿Está seguro de eliminar ' + descripcion + '? Esta acción no se puede deshacer.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then(function (resultado) {
                if (resultado.isConfirmed) {
                    formulario.submit();
                }
            });
        });

        // Evita registrar dos veces una operación de stock por doble clic.
        $(document).on('submit', '.form-operacion', function () {
            const formulario = $(this);
            if (formulario.data('validator') && !formulario.valid()) {
                return;
            }
            formulario.find('button[type=submit]').prop('disabled', true);
        });

        // Las alertas de éxito se cierran solas.
        setTimeout(function () {
            document.querySelectorAll('.alert-success.alert-dismissible').forEach(function (alerta) {
                bootstrap.Alert.getOrCreateInstance(alerta).close();
            });
        }, 6000);
    });
})(jQuery);
