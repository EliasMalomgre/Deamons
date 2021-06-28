package be.kdg.stemtest.view.fragments.customdebategame

import android.content.Context
import androidx.lifecycle.ViewModelProviders
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.activity.addCallback
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.viewmodel.CDgResultViewModel
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import javax.inject.Inject


class CDgResult : Fragment(),HasAndroidInjector {


    private lateinit var statementData : LiveData<List<Statement>>
    private lateinit var answerData: LiveData<List<StudentAnswer>>
    private lateinit var answerOptionData: LiveData<List<AnswerOption>>

    private lateinit var recyclerView: RecyclerView
    private lateinit var viewAdapter: CdGResultAdapter
    private lateinit var viewManager: LinearLayoutManager

    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory


    private lateinit var viewModel: CDgResultViewModel
    private var backPressed = false;

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.c_dg_result_fragment, container, false)
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
        viewModel = ViewModelProviders.of(this, viewModelFactory)[CDgResultViewModel::class.java]
        initialiseViews(view)
        getData()
        fillViews(view)

    }

    private fun getData() {

        answerOptionData=viewModel.getAnswerOptions()
        val answerOptionObserver= Observer<List<AnswerOption>>{
                s -> viewAdapter.setAnswerOptionsData(s)
        }
        answerOptionData.observe(viewLifecycleOwner,answerOptionObserver)

        answerData = viewModel.getAnswers()
        val answerObserver= Observer<List<StudentAnswer>>{
                s -> viewAdapter.setAnswerData(s)
        }
        answerData.observe(viewLifecycleOwner,answerObserver)

        statementData = viewModel.getStatements()
        val statementObserver = Observer<List<Statement>>{
                s -> viewAdapter.setStatementData(s)
        }
        statementData.observe(viewLifecycleOwner,statementObserver)
    }

    private fun initialiseViews(view: View) {
        recyclerView = view.findViewById(R.id.rv_stellingen_CDg)
        viewManager = LinearLayoutManager(context)
        viewAdapter =
            CdGResultAdapter(context)
    }

    private fun fillViews(view: View) {
        recyclerView.apply {
            setHasFixedSize(true)
            layoutManager=viewManager
            adapter=viewAdapter
        }
    }

    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }

    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }
}
