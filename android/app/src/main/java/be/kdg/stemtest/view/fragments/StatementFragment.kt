package be.kdg.stemtest.view.fragments

import android.annotation.SuppressLint
import android.content.Context
import android.graphics.Typeface
import android.os.Bundle
import android.text.SpannableString
import android.text.Spanned
import android.text.method.LinkMovementMethod
import android.text.style.ClickableSpan
import android.view.*
import android.widget.*
import androidx.activity.addCallback
import androidx.core.os.bundleOf
import androidx.fragment.app.Fragment
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.ViewModelProviders
import androidx.navigation.findNavController
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.model.entity.AnswerOption
import be.kdg.stemtest.model.entity.Definition
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Statement
import be.kdg.stemtest.viewmodel.DgStellingViewModel
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import kotlinx.android.synthetic.main.dg_stelling_fragment.view.*
import java.util.*
import javax.inject.Inject


class StatementFragment : Fragment(), HasAndroidInjector {


    private lateinit var viewModel: DgStellingViewModel
    private lateinit var progressBar: ProgressBar
    private lateinit var statement: TextView
    private lateinit var argument: TextView
    private lateinit var ll1: LinearLayout
    private lateinit var ll2: LinearLayout
    private lateinit var ll3: LinearLayout
    private lateinit var btn: Button
    private lateinit var infoBtn: ImageButton

    private var errorShown = false
    private var index: Int = 0

    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    private lateinit var answerOptionData: LiveData<List<AnswerOption>>
    private lateinit var statementData: LiveData<Statement>
    private var backPressed = false;

    private lateinit var gameData: LiveData<GameSettings>
    private lateinit var definitonData: LiveData<List<Definition>>

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.dg_stelling_fragment, container, false)
        val callback = requireActivity().onBackPressedDispatcher.addCallback(this) {
            if (!backPressed) {
                Toast.makeText(
                    context,
                    "Weet je zeker dat je het spel wil verlaten?",
                    Toast.LENGTH_LONG
                ).show();
                backPressed = true;
            } else {
                view.findNavController().navigate(R.id.connect);
            }
        }
        return view;
    }


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        viewModel = ViewModelProviders.of(this, viewModelFactory)[DgStellingViewModel::class.java]
        index = viewModel.getIndex()!!
        initialiseViews(view)
        fillViews(view)
    }

    fun setMarginsInDp(view: View, left: Int, top: Int, right: Int, bottom: Int) {
        if (view.layoutParams is ViewGroup.MarginLayoutParams) {
            val screenDesity: Float = view.context.resources.displayMetrics.density
            val params: ViewGroup.MarginLayoutParams =
                view.layoutParams as ViewGroup.MarginLayoutParams
            params.setMargins(
                left * screenDesity.toInt(),
                top * screenDesity.toInt(),
                right * screenDesity.toInt(),
                bottom * screenDesity.toInt()
            )
            view.requestLayout()
        }
    }

    @SuppressLint("ResourceType")
    private fun fillViews(v: View) {
        definitonData=viewModel.getDefinitions()
        val definitonsObserver= Observer<List<Definition>> {i ->
            val substring = statement.text.split(' ')
            for (s in substring) {
                val woord = i.find { i -> i.word.equals(s) }
                if (woord != null) {
                    val startIndex = statement.text.toString().indexOf(woord.word)
                    val endIndex = startIndex + woord.word.length
                    val spanString = SpannableString(statement.text)
                    val clickableSpan= object : ClickableSpan(){
                        override fun onClick(widget: View) {
                            val popupView: View = LayoutInflater.from(activity)
                                .inflate(R.layout.definition_popup, null)
                            val text = popupView.findViewById<TextView>(R.id.tvPopup)
                            text.text = woord.explanation
                            val popupWindow = PopupWindow(
                                popupView,
                                WindowManager.LayoutParams.MATCH_PARENT,
                                WindowManager.LayoutParams.MATCH_PARENT
                            )
                            text.setOnClickListener { v ->
                                popupWindow.dismiss();
                            }
                            popupWindow.showAsDropDown(popupView, 0, 0)
                        }
                    }
                    spanString.setSpan(
                        clickableSpan,
                        startIndex,
                        endIndex,
                        Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                    )
                    spanString.setSpan(clickableSpan,startIndex,endIndex, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE)
                    statement.text=spanString
                    statement.movementMethod=LinkMovementMethod.getInstance()
                }
            }
        }






        answerOptionData = viewModel.getAnswerOptions(index + 1)
        val answerOptionObserverSkipAllowed = Observer<List<AnswerOption>> { i ->
            var counter = 0
            var c1 = 0
            var c2 = 0
            for (antwoordMogelijkheid in i) {
                btn = Button(this.context)
                btn.layoutParams = LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.WRAP_CONTENT,
                    LinearLayout.LayoutParams.WRAP_CONTENT
                );
                btn.text = antwoordMogelijkheid.opinion
                btn.setPadding(40, 40, 40, 40)
                setMarginsInDp(btn, 15, 0, 15, 0)
                val font = Typeface.createFromAsset(context?.assets, "fonts/Allerta-Regular.ttf")
                btn.setTypeface(font)
                btn.setBackgroundResource(R.drawable.buttonlayout)

                btn.setOnClickListener {
                    val colour = counter
                    pushAnswer(v, antwoordMogelijkheid.id, colour)
                }

                if (antwoordMogelijkheid.id == 3) {
                    ll3.addView(btn, 0)
                    counter--
                } else if (counter < 3) {
                    ll1.addView(btn, c1)
                    c1++
                } else {
                    ll2.addView(btn, c2)
                    c2++
                }

                counter++
            }
        }

        val answerOptionObserverSkiNotAllowed = Observer<List<AnswerOption>> { i ->
            var counter = 0
            var c1 = 0
            var c2 = 0
            for (antwoordMogelijkheid in i) {
                btn = Button(this.context)
                btn.layoutParams = LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.WRAP_CONTENT,
                    LinearLayout.LayoutParams.WRAP_CONTENT
                );
                btn.text = antwoordMogelijkheid.opinion
                btn.setPadding(40, 40, 40, 40)
                setMarginsInDp(btn, 15, 0, 15, 0)
                val font = Typeface.createFromAsset(context?.assets, "fonts/Allerta-Regular.ttf")
                btn.setTypeface(font);
                btn.setBackgroundResource(R.drawable.buttonlayout)

                val colour = counter;
                btn.setOnClickListener {
                    pushAnswer(v, antwoordMogelijkheid.id, colour + 1)
                }

                if (antwoordMogelijkheid.opinion.toUpperCase()=="OVERSLAAN" ||antwoordMogelijkheid.opinion.toUpperCase()=="SKIP") {

                } else if (counter < 3) {
                    ll1.addView(btn, c1)
                    c1++
                } else {
                    ll2.addView(btn, c2)
                    c2++
                }
                counter++
            }
        }

        gameData = viewModel.getGameType()
        val gameSettingsObserver = Observer<GameSettings> { i ->
            if (!i.argumentsAllowed) {
                argument.visibility = View.GONE
            }
            //only observe the definitions if they are allowed
            if (i.definitionsGiven) {
                definitonData.observe(viewLifecycleOwner, definitonsObserver)
            }
            //different observers for if skip button should be added
            if (!i.skipAllowed){
                answerOptionData.observe(viewLifecycleOwner,answerOptionObserverSkiNotAllowed)
            } else{
                answerOptionData.observe(viewLifecycleOwner,answerOptionObserverSkipAllowed)
            }
        }


        statementData = viewModel.getStatement(index)
        val observer = Observer<Statement> { s ->
            if (s.id==-1){
                if (errorShown==false){
                    Toast.makeText(context,"Kon geen data ophalen",Toast.LENGTH_LONG).show()
                    errorShown=true
                }
            }else
                errorShown=false
            statement.text = s.text
            if (s.explanation==null) {
                infoBtn.visibility = View.GONE
            } else {
                infoBtn.setOnClickListener {
                    val popupView: View = LayoutInflater.from(activity)
                        .inflate(R.layout.definition_popup, null)
                    val text = popupView.findViewById<TextView>(R.id.tvPopup)
                    text.text = s.explanation
                    val popupWindow = PopupWindow(
                        popupView,
                        WindowManager.LayoutParams.MATCH_PARENT,
                        WindowManager.LayoutParams.MATCH_PARENT
                    )
                    text.setOnClickListener { v ->
                        popupWindow.dismiss();
                    }
                    popupWindow.showAsDropDown(popupView, 0, 0)
                }
            }
            //Chains the statement,gamesettings and definitonsobserver
            gameData.observe(viewLifecycleOwner, gameSettingsObserver)
        }
        statementData.observe(viewLifecycleOwner, observer)


    }


    private fun pushAnswer(view: View, answerOption: Int, colourIndex: Int) {

        val bundle = bundleOf("coloursArg" to colourIndex)
        if (argument.visibility == View.GONE) {

            viewModel.pushAnswer(" ", answerOption, index)
            val navController = view.findNavController()
            navController.navigate(R.id.wachtscherm, bundle)
        } else {
            val argumentText = argument.text.toString()
            if (!argumentText.equals(""))
            {
                viewModel.pushAnswer(argumentText, answerOption, index)
                val navController = view.findNavController()
                navController.navigate(R.id.wachtscherm, bundle)
            } else {
                Toast.makeText(context, "Je moet een argument meegeven!", Toast.LENGTH_SHORT).show()
            }
        }
    }

    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }

    fun initialiseViews(view: View) {
        progressBar = view.pbDgStelling
        progressBar.max = 100
        progressBar.progress = 50
        argument = view.findViewById(R.id.etxtDgArgument)
        statement = view.findViewById(R.id.txtDgStelling)
        infoBtn=view.findViewById(R.id.btnStatementInfo)



        ll1 = view.findViewById(R.id.llDgStellingen1) as LinearLayout
        ll2 = view.findViewById(R.id.llDgStellingen2) as LinearLayout
        ll3 = view.findViewById(R.id.llDgStellingen3) as LinearLayout
    }

    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }


}
