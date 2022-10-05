$(document).ready(function () {
    var MAX_INPUTTEXT_LENGTH  = 10000,
        LOCALSTORAGE_TEXT_KEY = 'syntax-ru-text',
        DEFAULT_TEXT          = 'В Петербурге перед судом предстанет высокопоставленный офицер Генерального штаба ВС РФ. СКР завершил расследование уголовного дела против главы военно-топографического управления Генштаба контр-адмирала Сергея Козлова, обвиняемого в превышении должностных полномочий и мошенничестве.\n' +
'"Следствием собрана достаточная доказательственная база, подтверждающая виновность контр-адмирала Козлова в инкриминируемых преступлениях, в связи с чем уголовное дело с утвержденным обвинительным заключением направлено в суд для рассмотрения по существу", - рассказали следователи.\n' +
'Кроме того, по инициативе следствия представителем Минобороны России к С.Козлову заявлен гражданский иск о возмещении причиненного государству ущерба на сумму свыше 27 млн руб.\n' +
'По данным следователей, в июле 2010г. военный чиновник отдал подчиненному "заведомо преступный приказ" о заключении лицензионных договоров с компаниями "Чарт-Пилот" и "Транзас". Им необоснованно были переданы права на использование в коммерческих целях навигационных морских карт, являвшихся интеллектуальной собственностью РФ. В результате ущерб составил более 9,5 млн руб.\n' +
'Контр-адмирал также умолчал о наличии у него в собственности квартиры в городе Истра Московской области. В результате в 2006г. центральной жилищной комиссии Минобороны и Управления делами президента РФ С.Козлов был признан нуждающимся в жилье и в 2008г. получил от государства квартиру в Москве площадью 72 кв. м и стоимостью 18,5 млн руб. Квартиру позднее приватизировала его падчерица.\n' +
'Против С. Козлова возбуждено дело по п."в" ч.3 ст.286 (превышение должностных полномочий, совершенное с причинением тяжких последствий) и ч.4 ст.159 (мошенничество, совершенное в особо крупном размере) УК РФ.\n' +
'\n' +
'(Скоро в российскую столицу придет Новый Год.)\n' +
'(На самом деле iPhone - это просто смартфон.)\n' +
'\n' +
'(пример частеречной омонимии:)\n' +
'Вася, маша руками и коля дрова, морочил голову.\n' +
'Вася, Маша и Коля пошли гулять.\n' + 
'\n' +
'Гло́кая ку́здра ште́ко будлану́ла бо́кра и курдя́чит бокрёнка.\n' +
'Варкалось. Хливкие шорьки пырялись по наве, и хрюкотали зелюки, как мюмзики в мове.';

    var textOnChange = function () {
        var _len = $('#text').val().length; 
        var len = _len.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
        var $textLength = $('#textLength');
        $textLength.html('длина текста: ' + len + ' символов');
        if (MAX_INPUTTEXT_LENGTH < _len) $textLength.addClass('max-inputtext-length');
        else                             $textLength.removeClass('max-inputtext-length');
    };
    var getText = function( $text ) {
        var text = trim_text( $text.val().toString() );
        if (is_text_empty(text)) {
            alert('Введите текст для обработки.');
            $text.focus();
            return (null);
        }
        
        if (text.length > MAX_INPUTTEXT_LENGTH) {
            if (!confirm('Превышен рекомендуемый лимит ' + MAX_INPUTTEXT_LENGTH + ' символов (на ' + (text.length - MAX_INPUTTEXT_LENGTH) + ' символов).\r\nТекст будет обрезан, продолжить?')) {
                return (null);
            }
            text = text.substr( 0, MAX_INPUTTEXT_LENGTH );
            $text.val( text );
            $text.change();
        }
        return (text);
    };

    $('#text').focus(textOnChange).change(textOnChange).keydown(textOnChange).keyup(textOnChange).select(textOnChange).focus();

    (function () {
        function isGooglebot() { return (navigator.userAgent.toLowerCase().indexOf('googlebot/') !== -1); };
        if (isGooglebot()) return;

        var text = localStorage.getItem(LOCALSTORAGE_TEXT_KEY);
        if (!text || !text.length) text = DEFAULT_TEXT;
        $('#text').val(text).focus();
    })();
    $('#resetText2Default').click(function () {
        $('#text').val('');
        setTimeout(function () { $('#text').val(DEFAULT_TEXT).focus(); }, 100);
    });

    $('#processButton').click(function () {
        if($(this).hasClass('disabled')) return (false);

        var text = getText( $('#text') );
        if (!text) return (false);

        processing_start();
        if (text !== DEFAULT_TEXT) {
            localStorage.setItem(LOCALSTORAGE_TEXT_KEY, text);
        } else {
            localStorage.removeItem(LOCALSTORAGE_TEXT_KEY);
        }

        var model = {
            splitBySmiles: true,
            text         : text
        };
        $.ajax({
            type       : 'POST',
            contentType: 'application/json',
            dataType   : 'json',
            url        : '/Process/Run',
            data       : JSON.stringify( model ),
            success: function (resp) {
                if (resp.err) {
                    if (resp.err === 'goto-on-captcha') {
                        window.location.href = '/Captcha/GetNew';
                    } else {
                        processing_end();
                        $('.result-info').addClass('error').text(resp.err);
                    }
                } else {
                    if (resp.sents && resp.sents.length) {
                        $('.result-info').removeClass('error').text('');
                        var html = detailedView(resp, text);
                        $('#processResult tbody').html(html);
                        $('.result-info').hide();
                        processing_end();
                    } else if (resp.html) {
                        $('.result-info').removeClass('error').text('');
                        $('#processResult tbody').html(resp.html);
                        processing_end();
                    } else {
                        processing_end();
                        $('.result-info').text('значимых сущностей в тексте не найденно');
                    }
                }
            },
            error: function () {
                processing_end();
                $('.result-info').text('ошибка сервера');
            }
        });
    });

    function detailedView(resp, text) {
        var header = "<tr>" +
                     "<td class='caption'>original-word</td>" +
                     "<td class='caption'>normal-form</td>" +
                     "<td class='caption'>syntax-role</td>" +
                     "<td class='caption'>part-of-speech</td>" +
                     "<td class='caption'>morpho-features</td>" +
                     "</tr>";
        var trs = [];
        trs[0] = header;

        for (var i = 0, len = resp.sents.length - 1; i <= len; i++) {
            var words_by_sent = resp.sents[ i ],
                wordFirst = words_by_sent[ 0 ],
                wordLast  = words_by_sent[ words_by_sent.length - 1 ],
                sentText  = text.substr(wordFirst.i, wordLast.i + wordLast.l - wordFirst.i),
                sentNumber = (i + 1),
                even_odd = (((sentNumber % 2) === 0) ? "even" : "odd");
            var tr = "<tr class='" + even_odd + "'>" +
                       "<td colspan='5'><span class='sent-number'>" + sentNumber + "). <i>" + sentText + "</i></span></td>" +
                     "</tr>";
            for (var j = 0, len_2 = words_by_sent.length; j < len_2; j++) {
                var word = words_by_sent[ j ],
                    wordValueOrigin = text.substr( word.i, word.l ); //word.v;
                tr += "<tr class='" + even_odd + "'> <td class='word'>" + wordValueOrigin + "</td>";
                var morpho = word.morpho;
                if (morpho) {
                    tr += "<td><b>" + (morpho.nf || wordValueOrigin) + "</b></td>" + 
                          "<td><span class='syntax " + word.stx + "'>" + word.stx + "</span></td>";
                    tr += "<td> <span class='" + morpho.pos + "-2'>" + morpho.pos + "</span>";
                    if (word.pos !== morpho.pos) {
                        tr += " |<span class='" + word.pos + "-2'>" + word.pos + "</span>";
                    }
                    tr += "</td>";
                    tr += "<td> <span class='MA'>" + morpho.ma + "</span> </td>";
                } else {
                    tr += "<td><b>" + wordValueOrigin + "</b></td>" + 
                          "<td><span class='syntax " + word.stx + "'>" + word.stx + "</span></td>";
                    if (word.p) {
                        tr += "<td><span class='O'> <span class='font-small'>(punctuation)</span> </span></td> <td><span class='MA'>-</span></td>";
                    } else {
                        tr += "<td><span class='" + word.pos + "-2'>" + word.pos + "</span></td> <td><span class='MA'>-</span></td>";
                    }
                }
                tr += "</tr>";
            }
            tr += "<tr><td colspan='4' /></tr>";
            //if (i != len) tr += header;
            trs[ sentNumber ] = tr;
        }
        return (trs.join(''));
    };

    function processing_start(){
        $('#text').addClass('no-change').attr('readonly', 'readonly').attr('disabled', 'disabled');
        $('.result-info').show().removeClass('error').html('Идет обработка... <label id="processingTickLabel"></label>');
        $('#processButton').addClass('disabled');
        $('#processResult tbody').empty();
        processingTickCount = 1; setTimeout(processing_tick, 1000);
    };
    function processing_end(){
        $('#text').removeClass('no-change').removeAttr('readonly').removeAttr('disabled');
        $('.result-info').removeClass('error').text('');
        $('#processButton').removeClass('disabled');
    };
    function trim_text(text) { return (text.replace(/(^\s+)|(\s+$)/g, '')); };
    function is_text_empty(text) { return (!trim_text(text)); };

    var processingTickCount = 1,
        processing_tick = function() {
            var n2 = function (n) {
                n = n.toString();
                return ((n.length === 1) ? ('0' + n) : n);
            }
            var d = new Date(new Date(new Date(new Date().setHours(0)).setMinutes(0)).setSeconds(processingTickCount));
            var t = n2(d.getHours()) + ':' + n2(d.getMinutes()) + ':' + n2(d.getSeconds()); //d.toLocaleTimeString();
            var $s = $('#processingTickLabel');
            if ($s.length) {
                $s.text(t);
                processingTickCount++;
                setTimeout(processing_tick, 1000);
            } else {
                processingTickCount = 1;
            }
        };
});
