$(document).ready(function () {
    var idCliente = 0;
    if (typeof obj !== 'undefined' && obj.Id > 0) {
        idCliente = obj.Id;    
    }


    $("#incluir").click(function () {
        var cpf = $("#cpfBeneficiario").val();
        var nome = $("#nomeBeneficiario").val();
        if (cpf && nome) {
            var beneficiario = {
                CPF: cpf,
                Nome: nome,
                Id: 0,
                idCliente: idCliente
            };

            $.ajax({
                url: '/Cliente/AddBeneficiario',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(beneficiario),
                success: function (r) {
                    if (r.success) {
                        atualizarTabela(r.data);
                        $("#cpfBeneficiario").val('');
                        $("#nomeBeneficiario").val('');
                    } else {
                        ModalDialog("Erro", r.message);
                    }



                },
                error: function (error) {
                    ModalDialog("Erro", r.message);
                }
            });
        } else {
            alert("Preencha todos os campos!");
        }
    });

    $("#beneficiarios").click(function () {
        if (typeof obj !== 'undefined' && obj.Id > 0) {
            carregarBeneficiarios(obj.Id);
        }
        $('#beneficiariosModal').modal('show');
    });    
   
    $('.cpf-mask').on('blur', function () {
        let value = $(this).val();
        value = value.replace(/\D/g, '');

        if (value.length === 11) {
            value = value.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
        }

        $(this).val(value);
    });
})

function atualizarTabela(beneficiarios) {
    var tbody = $("table tbody");
    tbody.empty();

    beneficiarios.forEach(function (b) {
        var row = "<tr>";
        row += "<td><input type='text' maxlength='14' value='" + b.CPF + "' id='cpf_" + b.Id + "' class='form-control cpf-mask'/></td>";
        row += "<td><input type='text' maxlength='50' value='" + b.Nome + "' id='nome_" + b.Id + "' class='form-control'/></td>";
        row += "<td><button onclick='alterarBeneficiario(\"" + b.Id + "\")' class='btn btn-primary btn-sm'>Alterar</button></td>";
        row += "<td><button type='submit' onclick='removerBeneficiario(\"" + b.Id + "\")' class='btn btn-primary btn-sm'>Remover</button></td>";
        row += "</tr>";
        tbody.append(row);
    });
}

function carregarBeneficiarios(idCli) {
    $.ajax({
        url: '/Beneficiario/BeneficiarioList',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ idCliente: idCli }),
        success: function (data) {
            atualizarTabela(data);
        },
        error: function (error) {
            ModalDialog("Erro", "Erro ao carregar beneficiários!");
        }
    });
}

function removerBeneficiario(id) {
    var cpf = $("#cpfBeneficiario").val();
    var nome = $("#nomeBeneficiario").val();
    var beneficiario = {
        CPF: cpf,
        Nome: nome,
        Id: id,
        idCliente: 0
    };    
    $.ajax({
        url: '/Cliente/RemoveBeneficiario',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(beneficiario),
        success: function (data) {
            ModalDialog("Exclusão", "Beneficiário excluído", function () {
                atualizarTabela(data);
                $('#beneficiariosModal').modal('hide');
            });
        },
        error: function (error) {
            console.log(error);
        }
    });
} 

function alterarBeneficiario(id) {
    var cpf = $("#cpf_" + id).val();
    var nome = $("#nome_" + id).val();
    $.ajax({
        url: '/Beneficiario/Alterar',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            "Id": id,
            "CPF": cpf,
            "Nome": nome
        }),
        success: function (r) {
            if (r.success) {
                ModalDialog("Sucesso!", r.message)
            } else {
                ModalDialog("Sucesso!", r.message);
            }
        },
        error: function (error) {
            if (r.status == 400)
                ModalDialog("Ocorreu um erro", r.responseJSON);
            else if (r.status == 500)
                ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            else
                ModalDialog("Ocorreu um erro", r.message);
        }
    });
}

function ModalDialog(titulo, texto, callback) {
    var random = Math.random().toString().replace('.', '');
    var modalHtml = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(modalHtml);
    var $modal = $('#' + random);
    $modal.modal('show');

    $modal.on('hidden.bs.modal', function () {
        if (callback) {
            callback();
        }
        $modal.remove();
    });
}
